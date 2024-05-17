// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PedGPT.Application.Commands;
using PedGPT.Core.Agents;
using PedGPT.Core.Json;
using PedGPT.Core.Memories;
using PedGPT.Core.OpenAi;
using PedGPT.Core.Prompts;

ILoggerFactory? loggerFactory = LoggerFactory.Create(builder => builder
    .AddConsole(options =>
    {
#pragma warning disable CS0618
        options.TimestampFormat = "[HH:mm:ss.fff] ";
#pragma warning restore CS0618
    })
    .SetMinimumLevel(LogLevel.Trace));

ILogger<Program>? logger = loggerFactory.CreateLogger<Program>();

IConfigurationBuilder? configBuilder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();

IConfigurationRoot? config = configBuilder.Build();

string? openAiApiKey = config["OpenAi:ApiKey"];

if (string.IsNullOrWhiteSpace(openAiApiKey))
    throw new Exception("OpenAi:ApiKey is not set in user secrets.");

logger.LogInformation("Hello.");

OpenAiService? openAiService = new OpenAiService(
    openAiApiKey, 
    new HttpClient(),
    loggerFactory.CreateLogger<OpenAiService>(),
    new SystemTextJsonSerializer());

Agent? agent = new AgentBuilder()
    .WithName("Brian")
    .WithGoal(new Goal("There is an injured player near you.", 1))
    .WithState(new AgentState("Health", "100/100"))
    .WithCommand<WalkCommand>()
    .WithCommand<DanceCommand>()
    .WithCommand<DriveToPositionCommand>()
    .WithCommand<EnterVehicleCommand>()
    .WithCommand<GpsCommand>()
    .Build(new MemoryStorage(), openAiService, loggerFactory.CreateLogger<Agent>(), new PromptGenerator(), new SystemTextJsonSerializer());

AgentRunner? agentRunner = new AgentRunner(agent, loggerFactory.CreateLogger<AgentRunner>());

await agentRunner.Run();

logger.LogInformation("Goodbye.");