using System.ComponentModel;
using System.Net.Http.Json;
using System.Numerics;
using IntelliPed.FiveM.Messages.Navigation;
using Microsoft.SemanticKernel;

namespace IntelliPed.Core.Plugins;

public class NavigationPlugin
{
    [KernelFunction]
    [Description("Navigates to the specified co-ordinate.")]
    public async Task<string> NavigateTo(
        Kernel kernel,
        [Description("The co-ordinate.")] Vector3 coordinate)
    {
        HttpClient httpClient = new();

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("http://localhost:5000/api/navigation/move-to-position", new MoveToPositionRequest
        {
            X = coordinate.X,
            Y = coordinate.Y,
            Z = coordinate.Z,
        });

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to navigate to ({coordinate.X}, {coordinate.Y}, {coordinate.Z})");
            return "Failed to navigate.";
        }

        Console.WriteLine($"Successfully navigated to ({coordinate.X}, {coordinate.Y}, {coordinate.Z})");
        return "Successfully navigated.";
    }

    [KernelFunction]
    [Description("Returns the co-ordinates of the specified location.")]
    [return: Description("The co-ordinates of the location in the format of: (x, y, z).")]
    public async Task<Vector3> GetLocation(
        Kernel kernel,
        [Description("The location.")] string location)
    {
        await Task.Delay(250);
        Console.WriteLine($"The co-ordinates of {location} are (12, 13, -44)");
        return new(12, 13, -44);
    }
}