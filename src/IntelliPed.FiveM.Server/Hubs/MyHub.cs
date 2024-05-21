using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace IntelliPed.FiveM.Server.Hubs;

public class MyHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await BaseScript.Delay(0);
        Debug.WriteLine($"From {user}: {message}");
        await Clients.Caller.SendAsync("ReceiveMessage", user, message);
    }
}