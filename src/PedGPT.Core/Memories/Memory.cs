using PedGPT.Core.Agents;
using PedGPT.Core.Commands;

namespace PedGPT.Core.Memories;

public class Memory : IMemory
{
    public List<ThinkResult> ThinkResults { get; init; } = new();
    public List<KeyValuePair<string, CommandResult>> CommandsExecuted { get; init; } = new ();
}