using PedGPT.Core.Agents;

namespace PedGPT.Core.Memories;

public class MemoryStorage : IMemoryStorage
{
    public List<Memory> Memories { get; init; } = new();

    public void Add(ThinkResult thinkResult, ActResult actResult)
    {
        Memories.Add(new Memory(thinkResult, actResult));
    }
}