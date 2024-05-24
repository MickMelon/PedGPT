using System;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using CitizenFX.Core;
using FxMediator.Server;
using IntelliPed.FiveM.Server.Services;
using IntelliPed.FiveM.Server.Util;
using IntelliPed.FiveM.Shared.Requests.Puppets;
using IntelliPed.FiveM.Shared.Requests.Navigation;
using IntelliPed.Messages;
using IntelliPed.Messages.Navigation;
using IntelliPed.Messages.Speech;

namespace IntelliPed.FiveM.Server.Hubs;

public class AgentHub : Hub<IAgentHub>, IAgentHub
{
    private readonly ServerMediator _mediator;
    private readonly BaseScriptProxy _baseScriptProxy;
    private readonly ConnectedAgentService _connectedAgentService;

    public AgentHub(
        ServerMediator mediator, 
        BaseScriptProxy baseScriptProxy, 
        ConnectedAgentService connectedAgentService)
    {
        _mediator = mediator;
        _baseScriptProxy = baseScriptProxy;
        _connectedAgentService = connectedAgentService;
    }

    public override async Task OnConnectedAsync()
    {
        await Functions.SwitchToMainThread();

        Player player = _baseScriptProxy.Players.First();

        CreatePuppetRpcReply reply = await _mediator.SendToClient(player, new CreatePuppetRpcRequest
        {
            X = 0f,
            Y = 0f,
            Z = 72f,
        });

        ConnectedAgent agent = new()
        {
            ConnectionId = Context.ConnectionId,
            PedNetworkId = reply.PedNetworkId
        };

        _connectedAgentService.Agents[Context.ConnectionId] = agent;

        Debug.WriteLine($"Agent connected: {agent}");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // TODO: Delete the ped.

        await base.OnDisconnectedAsync(exception);
    }

    public async Task MoveToPosition(MoveToPositionRequest request)
    {
        await Functions.SwitchToMainThread();

        Player player = _baseScriptProxy.Players.First();

        Debug.WriteLine($"Navigating to ({request.X}, {request.Y}, {request.Z})");

        ConnectedAgent agent = _connectedAgentService.Agents[Context.ConnectionId];

        _mediator.SendToClient(player, new MoveToPositionRpcRequest
        {
            PedNetworkId = agent.PedNetworkId,
            X = request.X,
            Y = request.Y,
            Z = request.Z
        });
    }

    public async Task Speak(SpeakRequest request)
    {
        await Functions.SwitchToMainThread();

        Player player = _baseScriptProxy.Players.First();

        Debug.WriteLine($"Saying: {request.Message}");

        ConnectedAgent agent = _connectedAgentService.Agents[Context.ConnectionId];

        BaseScript.TriggerClientEvent(player, "Speech", agent.PedNetworkId, request.Message);
    }

    public async Task FleeFrom(FleeFromRequest request)
    {
        await Functions.SwitchToMainThread();

        Player player = _baseScriptProxy.Players.First();

        Debug.WriteLine($"Fleeing from: {request.PedNetworkId}");

        ConnectedAgent agent = _connectedAgentService.Agents[Context.ConnectionId];

        BaseScript.TriggerClientEvent(player, "FleeFrom", agent.PedNetworkId, request.PedNetworkId);
    }
}