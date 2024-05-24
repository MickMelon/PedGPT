using IntelliPed.Core.Agents;
using IntelliPed.Core.Signals;
using IntelliPed.FiveM.Messages.Sensors;
using Microsoft.AspNetCore.SignalR.Client;

namespace IntelliPed.Core.Sensors;

public class SpeechSensor
{
    private readonly Agent _agent;

    public SpeechSensor(Agent agent)
    {
        _agent = agent;

        _agent.HubConnection.On<SpeechHeardEvent>("SpeechHeard", OnSpeechHeard);
    }

    private void OnSpeechHeard(SpeechHeardEvent @event)
    {
        _agent.HandleSignal(new SpeechSignal
        {
            Message = @event.Message
        });
    }
}