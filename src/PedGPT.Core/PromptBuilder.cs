using System.Text;

namespace PedGPT.Core;

public class PromptBuilder
{
    private readonly List<(string, string)> _commands = new();
    private readonly List<(string, string)> _states = new();
    private readonly List<string> _constraints = new();
    private readonly List<string> _performanceEvaluations = new();
    private readonly List<string> _memories = new();

    public PromptBuilder WithCommand(string name, string description)
    {
        _commands.Add((name, description));
        return this;
    }

    public PromptBuilder WithState(string name, string state)
    {
        _states.Add((name, state));
        return this;
    }

    public PromptBuilder WithConstraint(string constraint)
    {
        _constraints.Add(constraint);
        return this;
    }

    public PromptBuilder WithPerformanceEvaluation(string evaluation)
    {
        _performanceEvaluations.Add(evaluation);
        return this;
    }

    public PromptBuilder WithMemory(string memory)
    {
        _memories.Add(memory);
        return this;
    }

    public string Build()
    {
        var setup =
            """
            You are a ped in Grand Theft Auto V who is fully autonomous. Your goals are to freeroam. 

            Your decisions must always be made independently without seeking user assistance. 
            Play to your strengths as an LLM and pursue simple strategies with no legal complications.
            """;

        var constraints = new StringBuilder();
        constraints.AppendLine("Constraints:");
        foreach (var constraint in _constraints)
        {
            constraints.AppendLine($"- {constraint}");
        }

        var performanceEvaluations = new StringBuilder();
        performanceEvaluations.AppendLine("Performance Evaluations:");
        foreach (var evaluation in _performanceEvaluations)
        {
            performanceEvaluations.AppendLine($"- {evaluation}");
        }

        var commands = new StringBuilder();
        commands.AppendLine("Commands:");
        for (var i = 0; i < _commands.Count; i++)
        {
            var (name, description) = _commands[i];
            commands.AppendLine($"{i + 1}. {name}: \"{name}\", desc: \"{description}\"");
        }

        var states = new StringBuilder();
        states.AppendLine("Your Current State:");
        foreach (var (name, state) in _states)
        {
            states.AppendLine($"- {name}: {state}");
        }

        var memories = new StringBuilder();
        memories.AppendLine("Your Memories (last item is most recent):");
        foreach (var memory in _memories)
        {
            memories.AppendLine($"- {memory}");
        }

        var responseFormat =
            """
            You should only respond in JSON format as described below.
            Response Format:
            {
                "Thoughts": {
                    "Text": "thought",
                    "Reasoning": "reasoning",
                    "Plan": "- short bulleted\n- list that conveys\n- long-term plan",
                    "Criticism": "constructive self-criticism",
                    "Speak": "thoughts summary to say to user"
                },
                "Command": {
                    "Name": "command name",
                    "Args": {
                        "arg name": "value"
                    }
                }
            }
            Ensure the response can be parsed by System.Text.Json.JsonSerializer
            """;

        var prompt = new StringBuilder();

        prompt.AppendLine(setup);
        prompt.AppendLine(constraints.ToString());
        prompt.AppendLine(commands.ToString());
        prompt.AppendLine(states.ToString());
        prompt.AppendLine(performanceEvaluations.ToString());
        prompt.AppendLine(responseFormat);
        prompt.AppendLine();
        prompt.AppendLine(memories.ToString());

        return prompt.ToString();
    }
}