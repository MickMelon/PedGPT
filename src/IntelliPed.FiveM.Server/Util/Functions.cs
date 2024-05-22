using CitizenFX.Core;
using System.Threading.Tasks;

namespace IntelliPed.FiveM.Server.Util;

public static class Functions
{
    public static async Task SwitchToMainThread()
    {
        await BaseScript.Delay(0);
    }
}