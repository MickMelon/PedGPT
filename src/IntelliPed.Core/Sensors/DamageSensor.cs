using IntelliPed.Core.Agents;
using IntelliPed.Core.Signals;
using IntelliPed.FiveM.Messages.Sensors;
using Microsoft.AspNetCore.SignalR.Client;

namespace IntelliPed.Core.Sensors;

public class DamageSensor : Sensor
{
    private readonly Agent _agent;

    public DamageSensor(Agent agent)
    {
        _agent = agent;

        _agent.HubConnection.On<DamageReceivedEvent>("DamageReceived", OnDamageReceived);
    }

    private void OnDamageReceived(DamageReceivedEvent @event)
    {
        _agent.HandleSignal(new DamageSignal
        {
            DamageAmount = @event.PreviousHealth - @event.CurrentHealth,
            Source = "Unknown",
            Weapon = "Unknown",
        });
    }
}