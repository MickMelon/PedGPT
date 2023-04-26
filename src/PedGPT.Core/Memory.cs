using PedGPT.Core.Actions;

namespace PedGPT.Core;

public class Memory
{
    public List<ThinkResult> ThinkResults { get; set; } = new();
    public List<IAction> ActionsExecuted { get; set; } = new();
}