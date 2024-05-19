using FxMediator.Shared;

namespace IntelliPed.FiveM.Shared.Requests.Puppets;

public record CreatePuppetRpcRequest : IClientRequest<CreatePuppetRpcReply>
{
    public required float X { get; init; }
    public required float Y { get; init; }
    public required float Z { get; init; }
}

public record CreatePuppetRpcReply
{
    public required int PedNetworkId { get; init; }
}