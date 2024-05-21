namespace IntelliPed.Core.Signals;

public record DamageSignal : Signal
{
    public required int DamageAmount { get; init; }
    public required string Source { get; init; }
    public required string Weapon { get; init; }

    public override string ToString()
    {
        return $"You have received {DamageAmount} damage from {Source} using the weapon {Weapon}";
    }
}