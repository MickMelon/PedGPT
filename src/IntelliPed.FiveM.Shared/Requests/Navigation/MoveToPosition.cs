using FxMediator.Shared;

namespace IntelliPed.FiveM.Shared.Requests.Navigation;

public record MoveToPositionRpcRequest : IClientRequest
{
    public required int PedNetworkId { get; init; }
    public required float X { get; init; }
    public required float Y { get; init; }
    public required float Z { get; init; }
}