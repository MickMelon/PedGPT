namespace PedGPT.Core.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class CommandDescriptionAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public CommandDescriptionAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}