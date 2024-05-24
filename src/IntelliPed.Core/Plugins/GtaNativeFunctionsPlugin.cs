using System.ComponentModel;
using System.Diagnostics;
using Microsoft.SemanticKernel;

namespace IntelliPed.Core.Plugins;

public class GtaNativeFunctionsPlugin
{
    [KernelFunction]
    [Description("Executes a native GTA V function. Make use of your knowledge of native GTA V / FiveM functions.")]
    public async Task<string> ExecuteNativeFunction(
        Kernel kernel,
        [Description("The native hash e.g. GET_ENTITY_HEALTH, TASK_GO_TO_ENTITY, TASK_SMART_FLEE_PED")] string nativeHash,
        [Description("The arguments required for the native function.")] params object[] args)
    {
        Debug.WriteLine($"Used native hash {nativeHash} with args {string.Join(", ", args)}");
        return "Executed native.";
    }
}