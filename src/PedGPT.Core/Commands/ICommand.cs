namespace PedGPT.Core.Commands;

public interface ICommand
{
    Task<CommandResult> Execute();
}