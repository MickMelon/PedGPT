using PedGPT.Core.Commands;

namespace PedGPT.Application.Commands;

[CommandDescription("gps", "Gets the coordinates for a given location.")]
public record GpsCommand(string Location) : ICommand
{
    public Task<CommandResult> Execute()
    {
        return Task.FromResult(new CommandResult(true, $"The coordinates for '{Location}' are '50.43, 12.44, 4.44'"));
    }
}