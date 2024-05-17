using System.ComponentModel;
using System.Numerics;
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
        await Task.Delay(1000);
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