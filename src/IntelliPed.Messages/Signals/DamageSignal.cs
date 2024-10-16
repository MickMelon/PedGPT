namespace IntelliPed.Messages.Signals;

public record DamageSignal : Signal
{
    public required int DamageAmount { get; init; }
    public required int SourcePedNetworkId { get; init; }
    public required string Weapon { get; init; }

    public override string ToString() =>
        $"""
        You have been damaged!
        Damage amount: {DamageAmount}
        Source ped network ID: {SourcePedNetworkId}
        Weapon: {Weapon}
        """;
}