using System.CommandLine;
using System.Reflection;
using CliTemplate.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration =  new ConfigurationBuilder()
  .AddJsonFile($"appsettings.json")
  .Build();

var services = new ServiceCollection();

var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
  .Where(e => e.IsDefined(typeof(CommandAttribute), false));

var root = new RootCommand();

foreach (var commandType in commandTypes)
{
  var cmdInfo = (CommandAttribute)Attribute.GetCustomAttributes(commandType).First(e => e is CommandAttribute);
  var cmd = new Command(cmdInfo.Name, cmdInfo.Description);

  cmd.SetHandler(async () =>
  {
    services.AddSingleton(commandType);
    var provider = services.BuildServiceProvider();
    var commandService = provider.GetService(commandType);
    var executionMethod = commandType.GetMethods()
      .First(m => m.IsDefined(typeof(CommandHandlerAttribute), false));

    var task = (Task)executionMethod.Invoke(commandService, null);

    if (task != null) await task;
  });

  root.Add(cmd);
}

await root.InvokeAsync(args);
