using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using FxMediator.Server;
using IntelliPed.FiveM.Messages.Puppets;
using IntelliPed.FiveM.Server.Controllers.Shared;
using IntelliPed.FiveM.Shared.Requests.Puppets;
using Microsoft.AspNetCore.Mvc;

namespace IntelliPed.FiveM.Server.Controllers;

[Route("api/[controller]")]
public class PuppetController : AppController
{
    private readonly BaseScriptProxy _baseScriptProxy;
    private readonly ServerMediator _mediator;

    public PuppetController(BaseScriptProxy baseScriptProxy, ServerMediator mediator)
    {
        _baseScriptProxy = baseScriptProxy;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        await SwitchToMainThread();

        Player player = _baseScriptProxy.Players.First();

        CreatePuppetRpcReply reply = await _mediator.SendToClient(player, new CreatePuppetRpcRequest
        {
            X = 0f,
            Y = 0f,
            Z = 72f,
        });

        Debug.WriteLine($"Created ped with ID {reply.PedNetworkId}");

        return Ok(new CreatePuppetReply
        {
            PedNetworkId = reply.PedNetworkId,
        });
    }
}