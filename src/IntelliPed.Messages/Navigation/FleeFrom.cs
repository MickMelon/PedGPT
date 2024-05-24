namespace IntelliPed.Messages.Navigation;

public record FleeFromRequest
{
    public required int PedNetworkId { get; init; }    
}