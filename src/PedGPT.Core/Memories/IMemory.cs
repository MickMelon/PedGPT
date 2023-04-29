using PedGPT.Core.Agents;
using PedGPT.Core.Commands;

namespace PedGPT.Core.Memories;

public interface IMemory
{
    List<ThinkResult> ThinkResults { get; init; }
    List<KeyValuePair<string, CommandResult>> CommandsExecuted { get; init; }
}