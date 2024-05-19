using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CitizenFX.Core;
using IntelliPed.FiveM.Messages.Navigation;

namespace IntelliPed.FiveM.Server.Controllers;

[Route("api/[controller]")]
public class NavigationController : Controller
{
    [HttpPost("move-to-position")]
    public async Task<IActionResult> MoveToPosition([FromBody] MoveToPositionRequest request)
    {
        Debug.WriteLine($"Navigating to ({request.X}, {request.Y}, {request.Z})");

        await BaseScript.Delay(5000);

        return Ok();
    }
}