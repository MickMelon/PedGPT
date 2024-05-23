using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using CitizenFX.Core;
using FxMediator.Server;
using IntelliPed.FiveM.Messages;
using IntelliPed.FiveM.Messages.Navigation;
using IntelliPed.FiveM.Server.Extensions;
using IntelliPed.FiveM.Server.Util;
using IntelliPed.FiveM.Shared.Requests.Puppets;
using IntelliPed.FiveM.Shared.Requests.Navigation;

namespace IntelliPed.FiveM.Server.Hubs;

public class AgentHub : Hub<IAgentHub>, IAgentHub
{
    private readonly ServerMediator _mediator;
    private readonly BaseScriptProxy _baseScriptProxy;

    public AgentHub(ServerMediator mediator, BaseScriptProxy baseScriptProxy)
    {
        _mediator = mediator;
        _baseScriptProxy = baseScriptProxy;
    }

    public async Task CreatePuppet()
    {
        await Functions.SwitchToMainThread();

        Player player = _baseScriptProxy.Players.First();

        CreatePuppetRpcReply reply = await _mediator.SendToClient(player, new CreatePuppetRpcRequest
        {
            X = 0f,
            Y = 0f,
            Z = 72f,
        });

        Context.Items.Add("PedNetworkId", reply.PedNetworkId);

        await Groups.AddToGroupAsync(Context.ConnectionId, reply.PedNetworkId.ToString());

        Debug.WriteLine($"Created ped with ID {reply.PedNetworkId}");
    }

    public async Task MoveToPosition(MoveToPositionRequest request)
    {
        await Functions.SwitchToMainThread();

        Player player = _baseScriptProxy.Players.First();

        Debug.WriteLine($"Navigating to ({request.X}, {request.Y}, {request.Z})");

        _mediator.SendToClient(player, new MoveToPositionRpcRequest
        {
            PedNetworkId = Context.GetPedNetworkId(),
            X = request.X,
            Y = request.Y,
            Z = request.Z
        });
    }
}