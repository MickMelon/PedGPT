using PedGPT.Core.Agents;

namespace PedGPT.Core.Prompts;

public interface IPromptGenerator
{
    string Generate(Agent agent);
}