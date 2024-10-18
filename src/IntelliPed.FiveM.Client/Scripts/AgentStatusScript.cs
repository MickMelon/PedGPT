using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CitizenFX.Core;
using IntelliPed.FiveM.Client.Models;
using IntelliPed.Messages.AgentStatus;

namespace IntelliPed.FiveM.Client.Scripts;

public record AgentStatus
{
    public int PedNetworkId { get; set; }
    public EnAgentStatus Status { get; set; }
}

public class AgentStatusScript : BaseScript
{
    private ConcurrentDictionary<int, AgentStatus> _agentStatuses = [];

    [EventHandler("AgentStatus:Set")]
    public void OnSetThinking(int pedNetworkId, EnAgentStatus status)
    {
        if (_agentStatuses.TryGetValue(pedNetworkId, out AgentStatus agentStatus))
        {
            agentStatus.Status = status;
        }
        else
        {
            _agentStatuses[pedNetworkId] = new AgentStatus
            {
                PedNetworkId = pedNetworkId,
                Status = status,
            };
        }
    }

    [Tick]
    public Task OnTick()
    {
        foreach (AgentStatus agentStatus in _agentStatuses.Values)
        {
            string? text = agentStatus.Status switch
            {
                EnAgentStatus.Thinking => "\ud83e\udd14",
                EnAgentStatus.Acting => "\ud83e\udd39",
                EnAgentStatus.Observing => "\ud83d\udc40",
                _ => null,
            };

            if (text is null)
            {
                continue;
            }

            Ped ped = (Ped)Entity.FromNetworkId(agentStatus.PedNetworkId);
            PedTextDraw textDraw = new(ped, text, heightOffset: 0.25f);
            textDraw.Draw();
        }

        return Task.FromResult(0);
    }
}