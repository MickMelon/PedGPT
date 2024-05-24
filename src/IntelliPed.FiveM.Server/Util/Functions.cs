using CitizenFX.Core;
using System.Threading.Tasks;

namespace IntelliPed.FiveM.Server.Util;

public static class Functions
{
    public static async Task SwitchToMainThread()
    {
        await BaseScript.Delay(0);
    }

    public static void SendChatMessage(string message)
    {
        BaseScript.TriggerClientEvent("chat:addMessage", new
        {
            color = new[] { 255, 255, 255 },
            args = new[] { "[IntelliPed]", message }
        });
    }
}