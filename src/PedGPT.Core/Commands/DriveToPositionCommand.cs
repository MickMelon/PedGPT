namespace PedGPT.Core.Commands;

[CommandDescription("drive_to_position", "Drives to the specified coordinates.")]
public record DriveToPositionCommand(float X, float Y, float Z) : ICommand
{
    public Task<CommandResult> Execute()
    {
        return Task.FromResult(new CommandResult(true, $"You have driven to '{X}, {Y}, {Z}'"));
    }
}