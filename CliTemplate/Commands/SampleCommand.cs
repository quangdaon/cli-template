using CliTemplate.Attributes;

namespace CliTemplate.Commands;

[Command("sample", "Print a sample message.")]
public class SampleCommand
{
  [Option("--count", "Number of times to repeat.", Aliases = new[] { "-c" })]
  public int Count { get; set; } = 1;

  [Option("--greeting", "The greeting to display.", Aliases = new[] { "-g" })]
  public string Greeting { get; set; }

  [CommandHandler]
  public Task Execute()
  {
    if (!string.IsNullOrEmpty(Greeting)) Console.WriteLine(Greeting);

    for (var i = 0; i < Count; i++)
    {
      Console.WriteLine("This is a sample!");
    }

    return Task.FromResult(0);
  }
}