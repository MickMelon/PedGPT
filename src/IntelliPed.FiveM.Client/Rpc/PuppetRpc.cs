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

    private static CreatePuppetRpcReply OnCreatePuppet(CreatePuppetRpcRequest request)
    {
        Debug.WriteLine("Creating puppet...");

        int pedHandle = API.CreatePed(0, (uint)API.GetHashKey("csb_agent"), request.X, request.Y, request.Z, 0f, true, true);

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