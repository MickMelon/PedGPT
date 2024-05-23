namespace IntelliPed.FiveM.Messages.Sensors;

public record DamageReceivedEvent
{
    public required int PreviousHealth { get; init; }
    public required int CurrentHealth { get; init; }
}