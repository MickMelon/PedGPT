using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using IntelliPed.FiveM.Messages.Puppets;
using Microsoft.AspNetCore.Mvc;

namespace IntelliPed.FiveM.Server.Controllers;

[Route("api/[controller]")]
public class PuppetController : Controller
{
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        await BaseScript.Delay(0);

        int pedHandle = API.CreatePed(0, (uint) API.GetHashKey("csb_agent"), 0f, 0f, 72f, 0f, true, true);

        if (pedHandle == 0)
        {
            Debug.WriteLine("Failed to create ped.");
            return BadRequest("Failed to create ped.");
        }

        Ped ped = new(pedHandle);

        Debug.WriteLine($"Created ped with ID {ped.NetworkId}");

        return Ok(new CreatePuppetReply
        {
            PedNetworkId = ped.NetworkId,
        });
    }
}