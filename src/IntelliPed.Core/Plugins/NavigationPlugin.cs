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
    [Description("Navigates to the specified co-ordinates.")]
    public async Task<string> NavigateTo(
        Kernel kernel,
        [Description("The co-ordinates.")] Coordinates coordinates)
    {
        Agent agent = kernel.GetRequiredService<Agent>();

        await agent.HubConnection.InvokeAsync("MoveToPosition", new MoveToPositionRequest
        {
            X = coordinates.X,
            Y = coordinates.Y,
            Z = coordinates.Z,
        });

        Console.WriteLine($"Successfully navigated to ({coordinates.X}, {coordinates.Y}, {coordinates.Z})");
        return "Successfully navigated.";
    }

    [KernelFunction]
    [Description("Returns the co-ordinates of the specified location.")]
    [return: Description("The co-ordinates of the location in the format of: (x, y, z).")]
    public async Task<Coordinates> GetLocation(
        [Description("The location.")] string location)
    {
        await Task.Delay(250);
        Console.WriteLine($"The co-ordinates of {location} are (-831, 172, 70)");
        return new Coordinates(-831, 172, 70);
    }

    [KernelFunction]
    [Description("Flees from the specified ped.")]
    public async Task<string> FleeFrom(
        Kernel kernel,
        [Description("The network ID of the ped to flee from.")] int pedNetworkIdToFleeFrom)
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