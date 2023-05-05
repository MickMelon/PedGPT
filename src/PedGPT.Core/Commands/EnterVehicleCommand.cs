namespace PedGPT.Core.Commands;

[CommandDescription("enter_vehicle", "Enters a vehicle with the given ID.")]
public record EnterVehicleCommand(int Id) : ICommand
{
    public Task<CommandResult> Execute()
    {
        return Task.FromResult(new CommandResult(true, $"You have walked entered vehicle '{Id}'"));
    }
}