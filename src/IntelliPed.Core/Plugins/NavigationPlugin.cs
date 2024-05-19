using System.ComponentModel;
using System.Net.Http.Json;
using IntelliPed.Core.Agents;
using IntelliPed.FiveM.Messages.Navigation;
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
        HttpClient httpClient = new();

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("http://localhost:5000/api/navigation/move-to-position", new MoveToPositionRequest
        {
            PedNetworkId = agent.PedNetworkId,
            X = coordinates.X,
            Y = coordinates.Y,
            Z = coordinates.Z,
        });

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to navigate to ({coordinates.X}, {coordinates.Y}, {coordinates.Z})");
            return "Failed to navigate.";
        }

        Console.WriteLine($"Successfully navigated to ({coordinates.X}, {coordinates.Y}, {coordinates.Z})");
        return "Successfully navigated.";
    }

    [KernelFunction]
    [Description("Returns the co-ordinates of the specified location.")]
    [return: Description("The co-ordinates of the location in the format of: (x, y, z).")]
    public async Task<Coordinates> GetLocation(
        Kernel kernel,
        [Description("The location.")] string location)
    {
        await Task.Delay(250);
        Console.WriteLine($"The co-ordinates of {location} are (-831, 172, 70)");
        return new Coordinates(-831, 172, 70);
    }
}

public class Coordinates
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Coordinates(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}