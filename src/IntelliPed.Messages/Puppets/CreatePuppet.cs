namespace IntelliPed.Messages.Puppets;

public record CreatePuppetReply
{
    public required int PedNetworkId { get; init; }
}