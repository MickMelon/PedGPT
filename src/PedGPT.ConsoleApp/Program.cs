// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PedGPT.Application.Commands;
using PedGPT.Core.Agents;
using PedGPT.Core.Json;
using PedGPT.Core.Memories;
using PedGPT.Core.OpenAi;
using PedGPT.Core.Prompts;

var loggerFactory = LoggerFactory.Create(builder => builder
    .AddConsole(options =>
    {
#pragma warning disable CS0618
        options.TimestampFormat = "[HH:mm:ss.fff] ";
#pragma warning restore CS0618
    })
    .SetMinimumLevel(LogLevel.Trace));

var logger = loggerFactory.CreateLogger<Program>();

var configBuilder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();

var config = configBuilder.Build();

var openAiApiKey = config["OpenAi:ApiKey"];

if (string.IsNullOrWhiteSpace(openAiApiKey))
    throw new Exception("OpenAi:ApiKey is not set in user secrets.");

logger.LogInformation("Hello.");

var openAiService = new OpenAiService(
    openAiApiKey, 
    new HttpClient(),
    loggerFactory.CreateLogger<OpenAiService>(),
    new SystemTextJsonSerializer());

var agent = new AgentBuilder()
    .WithName("Brian")
    .WithGoal(new Goal("There is an injured player near you.", 1))
    .WithState(new AgentState("Health", "100/100"))
    .WithCommand<WalkCommand>()
    .WithCommand<DanceCommand>()
    .WithCommand<DriveToPositionCommand>()
    .WithCommand<EnterVehicleCommand>()
    .WithCommand<GpsCommand>()
    .Build(new MemoryStorage(), openAiService, loggerFactory.CreateLogger<Agent>(), new PromptGenerator(), new SystemTextJsonSerializer());

var agentRunner = new AgentRunner(agent, loggerFactory.CreateLogger<AgentRunner>());

await agentRunner.Run();

logger.LogInformation("Goodbye.");