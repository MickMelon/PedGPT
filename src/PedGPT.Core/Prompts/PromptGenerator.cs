using PedGPT.Core.Agents;

namespace PedGPT.Core.Prompts;

public class PromptGenerator : IPromptGenerator
{
    private readonly string _setup =
        """
        You are a ped in Grand Theft Auto V who is fully autonomous. Your goals are to freeroam. 

        Your decisions must always be made independently without seeking user assistance. 
        Play to your strengths as an LLM and pursue simple strategies with no legal complications.
        """;

    private readonly string _responseFormat =
        """
        You must strictly ONLY respond in JSON format as described below.
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

    private readonly string[] _performanceEvaluations = 
    {
        //"Continuously review and analyze your actions to ensure you are performing to the best of your abilities.",
        //"Constructively self-criticize your big-picture behavior constantly.",
        //"Reflect on past decisions and strategies to refine your approach.",
        //"Be aware of your surroundings and the consequences of your actions.",
        //"Every command has a cost, so be smart and efficient. Aim to complete tasks in the least number of steps."
    };

    private readonly string[] _constraints =
    {
        "Must not use external APIs.",
        "Only use resources provided.",
        "Only one command can be used per resource.",
        "Exclusively use the commands listed in double quotes e.g. \"command name\"",
        "If you are unsure how you previously did something or want to recall past events, thinking about similar events will help you remember.",
};

    public string Generate(Agent agent)
    {
        PromptBuilder? promptBuilder = new PromptBuilder();

        promptBuilder.WithSetup($"Your name is {agent.Name}. {_setup}");
        promptBuilder.WithResponseFormat(_responseFormat);
        promptBuilder.WithPerformanceEvaluations(_performanceEvaluations);
        promptBuilder.WithConstraints(_constraints);

        agent.Goals.ForEach(_ => promptBuilder.WithGoal(_.Description, _.Importance));
        agent.States.ForEach(_ => promptBuilder.WithState(_.Name, _.State));
        agent.Commands.ForEach(_ => promptBuilder.WithCommand(_.Name, _.Description ?? "", _.ArgsDescriptions));
        //promptBuilder.WithMemories(agent.MemoryStorage.ThinkResults.Select(_ => JsonSerializer.Serialize(_)));

        string? prompt = promptBuilder.Build();

        return prompt;
    }
}