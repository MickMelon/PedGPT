using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace IntelliPed.FiveM.Client;

public class Program : BaseScript
{
    [EventHandler("onClientResourceStart")]
    public void OnClientResourceStart(string resourceName)
    {
        if (API.GetCurrentResourceName() != resourceName)
        {
            return;
        }

        Debug.WriteLine("Client resource started");

        API.SetEntityCoords(API.PlayerPedId(), 0f, 0f, 72f, false, false, false, false);
    }
}