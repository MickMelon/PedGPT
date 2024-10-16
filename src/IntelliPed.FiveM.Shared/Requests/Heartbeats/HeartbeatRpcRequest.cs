using System.Collections.Generic;
using FxMediator.Shared;
using IntelliPed.Messages.Heartbeats;

namespace IntelliPed.FiveM.Shared.Requests.Heartbeats;

public record HeartbeatRpcRequest : IClientRequest<HeartbeatRpcReply>
{
    public required int PedNetworkId { get; init; }
}

public record HeartbeatRpcReply
{
    public required string StreetName { get; init; }
    public required int Health { get; init; }
    public required List<NearbyPed> NearbyPeds { get; init; }
    public required List<NearbyVehicle> NearbyVehicles { get; init; }
}
