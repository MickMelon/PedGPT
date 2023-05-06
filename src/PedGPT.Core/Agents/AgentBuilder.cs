using Microsoft.Extensions.Logging;
using PedGPT.Core.Commands;
using PedGPT.Core.Json;
using PedGPT.Core.Memories;
using PedGPT.Core.OpenAi;
using PedGPT.Core.Prompts;

namespace PedGPT.Core.Agents;

public class AgentBuilder
{
    private string _name { get; set; } = "";
    private List<Goal> _goals { get; } = new();
    private List<AgentState> _states { get; } = new();
    private List<CommandDescriptor> _commands { get; } = new();

    public AgentBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public AgentBuilder WithGoal(Goal goal)
    {
        _goals.Add(goal);
        return this;
    }

    public AgentBuilder WithState(AgentState state)
    {
        _states.Add(state);
        return this;
    }

    public AgentBuilder WithCommand<T>() where T : ICommand
    {
        _commands.Add(CommandDescriptor.Create<T>());
        return this;
    }

    public Agent Build(
        IMemoryStorage memory,
        IOpenAiService openAiService,
        ILogger<Agent> logger,
        IPromptGenerator promptGenerator,
        IJsonSerializer jsonSerializer)
    {
        return new(
            _name, 
            _goals, 
            _states,
            _commands,
            memory,
            openAiService,
            logger,
            promptGenerator,
            jsonSerializer);
    }
}