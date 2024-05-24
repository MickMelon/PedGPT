using Microsoft.AspNetCore.SignalR;

namespace IntelliPed.FiveM.Server.Extensions;

public static class HubCallerContextExtensions
{
    public static int GetPedNetworkId(this HubCallerContext context)
    {
        return (int)context.Items["PedNetworkId"];
    }
}