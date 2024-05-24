using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FxMediator.Client;
using IntelliPed.FiveM.Shared.Requests.Puppets;

namespace IntelliPed.FiveM.Client.Rpc;

public class PuppetRpc : BaseScript
{
    private readonly ClientMediator _mediator = new();

    public PuppetRpc()
    {
        _mediator.AddRequestHandler<CreatePuppetRpcRequest, CreatePuppetRpcReply>(OnCreatePuppet);
    }

    private static async Task<CreatePuppetRpcReply> OnCreatePuppet(CreatePuppetRpcRequest request)
    {
        Debug.WriteLine("Creating puppet...");

        int count = 0;
        while (!API.HasModelLoaded((uint)API.GetHashKey("csb_agent")) && count < 100)
        {
            API.RequestModel((uint)API.GetHashKey("csb_agent"));
            await Delay(10);
            count++;
        }

        int pedHandle = API.CreatePed(0, (uint)API.GetHashKey("csb_agent"), request.X, request.Y, request.Z, 0f, true, false);

        if (pedHandle == 0)
        {
            Debug.WriteLine("ERROR: Failed to create ped!");
        }

        Ped ped = new(pedHandle)
        {
            BlockPermanentEvents = true,
            AlwaysKeepTask = true,
            IsPersistent = true,
        };

        ped.SetConfigFlag(429, true);

        API.SetEntityAsMissionEntity(ped.Handle, true, false);

        return new()
        {
            PedNetworkId = ped.NetworkId
        };
    }
}