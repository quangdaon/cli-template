using CliTemplate.Attributes;

namespace CliTemplate.Commands;

[Command("sample", "Print a sample message.")]
public class SampleCommand
{
  [CommandHandler]
  public Task Execute()
  {
    Console.WriteLine($"This is a sample!");
    return Task.FromResult(0);
  }
}