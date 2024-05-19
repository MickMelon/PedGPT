using System.Threading.Tasks;
using CitizenFX.Core;
using Microsoft.AspNetCore.Mvc;

namespace IntelliPed.FiveM.Server.Controllers.Shared;

public abstract class AppController : Controller
{
    public async Task SwitchToMainThread()
    {
        await BaseScript.Delay(0);
    }
}