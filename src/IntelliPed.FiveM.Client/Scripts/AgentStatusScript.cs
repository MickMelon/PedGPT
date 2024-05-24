using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CitizenFX.Core;
using IntelliPed.FiveM.Client.Models;

namespace IntelliPed.FiveM.Client.Scripts;

public record AgentStatus
{
    public int PedNetworkId { get; set; }
    public bool IsThinking { get; set; }
}

public class AgentStatusScript : BaseScript
{
    private ConcurrentDictionary<int, AgentStatus> _agentStatuses = [];

    [EventHandler("AgentStatus:SetThinking")]
    public void OnSetThinking(int pedNetworkId, bool isThinking)
    {
        if (_agentStatuses.TryGetValue(pedNetworkId, out AgentStatus agentStatus))
        {
            agentStatus.IsThinking = isThinking;
        }
        else
        {
            _agentStatuses[pedNetworkId] = new AgentStatus
            {
                PedNetworkId = pedNetworkId,
                IsThinking = isThinking
            };
        }
    }

    [Tick]
    public Task OnTick()
    {
        foreach (AgentStatus agentStatus in _agentStatuses.Values)
        {
            if (agentStatus.IsThinking)
            {
                Ped ped = (Ped)Entity.FromNetworkId(agentStatus.PedNetworkId);
                PedTextDraw textDraw = new(ped, "\ud83e\udd14", heightOffset: 0.25f);
                textDraw.Draw();
            }
        }

        return Task.FromResult(0);
    }
}