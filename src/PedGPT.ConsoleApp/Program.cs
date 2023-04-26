// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PedGPT.Core;
using PedGPT.Core.Actions;
using PedGPT.Core.OpenAi;

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
    loggerFactory.CreateLogger<OpenAiService>());

var agent = new Agent(new Memory(), openAiService, loggerFactory.CreateLogger<Agent>());

var actionMap = new Dictionary<string, Type>
{
    { "dance", typeof(DanceAction) },
    { "walk", typeof(WalkAction) },
};

var agentRunner = new AgentRunner(
    agent,
    new ActionLocator(actionMap), 
    loggerFactory.CreateLogger<AgentRunner>());

await agentRunner.Run();

logger.LogInformation("Goodbye.");