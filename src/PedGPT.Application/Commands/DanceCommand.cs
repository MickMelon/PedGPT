using PedGPT.Core.Commands;

namespace PedGPT.Application.Commands;

[CommandDescription("dance", "Allows you to dance for joy.")]
public record DanceCommand : ICommand
{
    public Task<CommandResult> Execute()
    {
        return Task.FromResult(new CommandResult(true, "You are now busting some moves like Michael Jackson!"));
    }
}