using CitizenFX.Core;
using CitizenFX.Core.Native;
using FxMediator.Client;
using IntelliPed.FiveM.Shared.Requests.Navigation;
using System.Threading.Tasks;

namespace IntelliPed.FiveM.Client.Rpc;

public class NavigationRpc : BaseScript
{
    private readonly ClientMediator _mediator = new();

    public NavigationRpc()
    {
        _mediator.AddRequestHandler<MoveToPositionRpcRequest>(OnMoveToPosition);
    }

    private static async Task OnMoveToPosition(MoveToPositionRpcRequest request)
    {
        Debug.WriteLine("Moving to position...");

        await RequestControlOfEntity(request.PedNetworkId);

        Ped ped = new(API.NetworkGetEntityFromNetworkId(request.PedNetworkId));
        
        API.TaskGoToCoordAnyMeansExtraParams(ped.Handle, request.X, request.Y, request.Z, 3f, 0, false, 786603, 0f, 0, 0, 0);
    }

    private static async Task RequestControlOfEntity(int networkId)
    {
        int controlCount = 0;

        while (!API.NetworkHasControlOfNetworkId(networkId) && controlCount < 10)
        {
            API.NetworkRequestControlOfNetworkId(networkId);
            controlCount++;
            await Delay(10);
        }

        if (controlCount >= 10)
        {
            Debug.WriteLine($"Unable to get control of entity {networkId}");
        }
    }
}