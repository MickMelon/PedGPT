using CitizenFX.Core.Native;
using CitizenFX.Core;
using FxMediator.Client;
using IntelliPed.FiveM.Shared.Requests.Puppets;
using System.Threading.Tasks;

namespace IntelliPed.FiveM.Client.Scripts;

public class PuppetScript : BaseScript
{
    private readonly ClientMediator _mediator = new();

    public PuppetScript()
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

        Ped ped = await World.CreatePed(new Model(PedHash.Agent), new Vector3(request.X, request.Y, request.Z));

        // Trying to ensure the ped doesn't despawn because it has a habit of doing so...
        ped.BlockPermanentEvents = true;
        ped.AlwaysKeepTask = true;
        ped.IsPersistent = true;
        ped.SetConfigFlag(429, true);

        API.SetEntityAsMissionEntity(ped.Handle, true, false);
        API.SetPedAsGroupLeader(ped.Handle, API.GetPedGroupIndex(API.PlayerPedId()));
        API.SetNetworkIdExistsOnAllMachines(ped.NetworkId, true);
        API.NetworkRegisterEntityAsNetworked(ped.Handle);
        API.SetEntityVisible(ped.Handle, true, false);
        API.NetworkRequestControlOfEntity(ped.Handle);

        // ReSharper disable once ObjectCreationAsStatement
        new Blip(API.AddBlipForEntity(ped.Handle))
        {
            Sprite = BlipSprite.ComedyClub,
            Scale = 0.5f
        };

        return new()
        {
            PedNetworkId = ped.NetworkId
        };
    }
}