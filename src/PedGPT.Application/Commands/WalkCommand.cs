using PedGPT.Core.Commands;

namespace PedGPT.Application.Commands;

[CommandDescription("walk", "Walks to the specified coordinates.")]
public record WalkCommand(float X, float Y, float Z) : ICommand
{
    public Task<CommandResult> Execute()
    {
        return Task.FromResult(new CommandResult(true, $"You have walked to '{X}, {Y}, {Z}'"));
    }
}