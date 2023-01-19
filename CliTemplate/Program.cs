using System.CommandLine;
using System.Reflection;
using CliTemplate.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Option CreateOption(PropertyInfo propertyInfo)
{
  var optType = typeof(Option<>);
  var optInfo = (OptionAttribute)Attribute.GetCustomAttributes(propertyInfo).First(e => e is OptionAttribute);
  var optPropType = propertyInfo.PropertyType;
  var genericType = optType.MakeGenericType(optPropType);
  var opt = (Option)Activator.CreateInstance(genericType, optInfo.Name, optInfo.Description)!;

  foreach (var alias in optInfo.Aliases)
  {
    opt.AddAlias(alias);
  }

  return opt;
}

Command CreateCommand(Type type, ServiceCollection serviceCollection)
{
  var cmdInfo = (CommandAttribute)Attribute.GetCustomAttributes(type).First(e => e is CommandAttribute);
  var command = new Command(cmdInfo.Name, cmdInfo.Description);

  var optionProps = type.GetProperties()
    .Where(e => e.IsDefined(typeof(OptionAttribute))).ToArray();

  var options = optionProps.Select(CreateOption).ToArray();
  foreach (var opt in options) command.Add(opt!);

  command.SetHandler(async (context) =>
  {
    serviceCollection.AddSingleton(type);
    var provider = serviceCollection.BuildServiceProvider();
    var commandService = provider.GetService(type);
    var executionMethod = type.GetMethods()
      .First(m => m.IsDefined(typeof(CommandHandlerAttribute), false));

    for (var i = 0; i < optionProps.Length; i++)
    {
      var optionProp = optionProps[i];
      var option = options[i];
      var value = context.ParseResult.GetValueForOption(option);
      optionProp.SetValue(commandService, value);
    }

    var task = (Task)executionMethod.Invoke(commandService, null);

    if (task != null) await task;
  });
  return command;
}

var configuration = new ConfigurationBuilder()
  .AddJsonFile($"appsettings.json")
  .Build();

var services = new ServiceCollection();

var root = new RootCommand();

var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
  .Where(e => e.IsDefined(typeof(CommandAttribute), false));

foreach (var commandType in commandTypes)
{
  var cmd = CreateCommand(commandType, services);
  root.Add(cmd);
}

await root.InvokeAsync(args);