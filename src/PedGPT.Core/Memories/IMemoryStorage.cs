using PedGPT.Core.Agents;

namespace PedGPT.Core.Memories;

public interface IMemoryStorage
{
    List<Memory> Memories { get; }
    void Add(ThinkResult thinkResult, ActResult actResult);
}