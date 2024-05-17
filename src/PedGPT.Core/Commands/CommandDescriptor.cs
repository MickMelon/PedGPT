using System.Reflection;

namespace PedGPT.Core.Commands;

public record CommandDescriptor(
    Type CommandType,
    string Name,
    string? Description,
    Dictionary<string, string>? ArgsDescriptions)
{
    public ICommand ToCommand(Dictionary<string, string> args)
    {
        ConstructorInfo? constructorInfo = CommandType.GetConstructors().First();
        ParameterInfo[]? parameters = constructorInfo.GetParameters();
        object[]? parameterValues = parameters.Select(p => Convert.ChangeType(args[p.Name!], p.ParameterType)).ToArray();

        return (ICommand)constructorInfo.Invoke(parameterValues);
    }

    public static CommandDescriptor Create<TCommand>() where TCommand : ICommand
    {
        Type? commandType = typeof(TCommand);

        (string? name, string? description) = ExtractCommandDescription(commandType);

        CommandDescriptor? commandDescriptor = new CommandDescriptor(
            typeof(TCommand),
            name,
            description,
            ExtractArgsDescription(commandType));

        return commandDescriptor;
    }

    private static (string, string) ExtractCommandDescription(Type commandType)
    {
        CommandDescriptionAttribute? attribute = commandType.GetCustomAttribute<CommandDescriptionAttribute>();

        if (attribute is null)
            throw new InvalidOperationException($"Command '{commandType.Name}' does not have a CommandDescriptionAttribute.");

        return (attribute.Name, attribute.Description);
    }

    private static Dictionary<string, string> ExtractArgsDescription(Type commandType)
    {
        ConstructorInfo? constructorInfo = commandType.GetConstructors().First();
        ParameterInfo[]? parameters = constructorInfo.GetParameters();

        return parameters.ToDictionary(_ => _.Name!, _ => _.ParameterType.ToString());
    }
}