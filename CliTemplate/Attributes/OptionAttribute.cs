namespace CliTemplate.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class OptionAttribute : Attribute
{
  public string Name { get; }
  public string Description { get; }
  public string[] Aliases { get; set; } = { };

  public OptionAttribute(string name, string description)
  {
    Name = name;
    Description = description;
  }
}