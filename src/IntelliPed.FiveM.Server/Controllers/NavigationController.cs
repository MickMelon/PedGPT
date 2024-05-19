using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CitizenFX.Core;
using FxMediator.Server;
using IntelliPed.FiveM.Messages.Navigation;
using IntelliPed.FiveM.Server.Controllers.Shared;
using IntelliPed.FiveM.Shared.Requests.Navigation;

namespace IntelliPed.FiveM.Server.Controllers;

[Route("api/[controller]")]
public class NavigationController : AppController
{
    private readonly BaseScriptProxy _baseScriptProxy;
    private readonly ServerMediator _mediator;

    public NavigationController(BaseScriptProxy baseScriptProxy, ServerMediator mediator)
    {
        _baseScriptProxy = baseScriptProxy;
        _mediator = mediator;
    }

    [HttpPost("move-to-position")]
    public async Task<IActionResult> MoveToPosition([FromBody] MoveToPositionRequest request)
    {
        await SwitchToMainThread();

        Player player = _baseScriptProxy.Players.First();

        Debug.WriteLine($"Navigating to ({request.X}, {request.Y}, {request.Z})");

        _mediator.SendToClient(player, new MoveToPositionRpcRequest
        {
            PedNetworkId = request.PedNetworkId,
            X = request.X,
            Y = request.Y,
            Z = request.Z
        });
        
        return Ok();
    }
}