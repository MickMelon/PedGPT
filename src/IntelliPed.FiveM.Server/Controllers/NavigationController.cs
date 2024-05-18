using CitizenFX.Core.Native;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IntelliPed.FiveM.Messages.Navigation;

namespace IntelliPed.FiveM.Server.Controllers;

[Route("api/[controller]")]
public class NavigationController : Controller
{
    [HttpPost("move-to-position")]
    public async Task<IActionResult> MoveToPosition([FromBody] MoveToPositionRequest request)
    {
        string? resourceName = null;
        TaskCompletionSource<bool> taskCompletionSource = new();

        GameScript.Instance.ExecuteOnGameThread(() =>
        {
            resourceName = API.GetCurrentResourceName();
            taskCompletionSource.SetResult(true);
        });

        await taskCompletionSource.Task;

        return Ok();
    }
}