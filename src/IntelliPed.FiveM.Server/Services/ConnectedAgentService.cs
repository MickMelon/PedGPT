using System.Collections.Concurrent;
using System.Linq;

namespace IntelliPed.FiveM.Server.Services;

public class ConnectedAgentService
{
    public ConcurrentDictionary<string, ConnectedAgent> Agents { get; } = [];

    public bool TryGetByPedNetworkId(int pedNetworkId, out ConnectedAgent? agent)
    {
        agent = Agents.Values.FirstOrDefault(agent => agent.PedNetworkId == pedNetworkId);
        return agent != null;
    }
}

public record ConnectedAgent
{
    public required string ConnectionId { get; init; }
    public required int PedNetworkId { get; init; }
}