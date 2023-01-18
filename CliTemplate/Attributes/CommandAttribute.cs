﻿namespace CliTemplate.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
  public string Name { get; set; }
  public string Description { get; set; }

  public CommandAttribute(string name, string description)
  {
    Name = name;
    Description = description;
  }
}