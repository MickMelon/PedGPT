using System.ComponentModel;
using IntelliPed.Core.Agents;
using IntelliPed.Messages.Common;
using IntelliPed.Messages.Navigation;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.SemanticKernel;

namespace IntelliPed.Core.Plugins;

public class NavigationPlugin
{
    [KernelFunction]
    [Description("Navigates to the specified coords.")]
    public static async Task<string> NavigateTo(Kernel kernel, Coordinates coords)
    {
        Agent agent = kernel.GetRequiredService<Agent>();

        await agent.HubConnection.InvokeAsync("MoveToPosition", new MoveToPositionRequest
        {
            X = coords.X,
            Y = coords.Y,
            Z = coords.Z,
        });

        Console.WriteLine($"Successfully navigated to {coords}.");
        return $"Successfully navigated to {coords}.";
    }

    [KernelFunction]
    [Description("Gets the coords for a location.")]
    [return: Description("Coords: (x, y, z)")]
    public static async Task<Coordinates> GetLocation(string locationName)
    {
        await Task.Delay(250);
        Console.WriteLine($"The co-ordinates of '{locationName}' are (-831, 172, 70)");
        return new Coordinates(-831, 172, 70);
    }

    [KernelFunction]
    [Description("Flees from a ped.")]
    public static async Task<string> FleeFrom(Kernel kernel, int pedNetworkIdToFleeFrom)
    {
        Agent agent = kernel.GetRequiredService<Agent>();

        await agent.HubConnection.InvokeAsync("FleeFrom", new FleeFromRequest
        {
            PedNetworkId = pedNetworkIdToFleeFrom,
        });

        Console.WriteLine($"Fleeing from {pedNetworkIdToFleeFrom}");
        return $"You have started fleeing from the ped with network id {pedNetworkIdToFleeFrom}";
    }
}