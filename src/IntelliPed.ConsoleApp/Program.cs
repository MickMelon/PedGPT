using System.Net.Http.Json;
using IntelliPed.Core.Agents;
using IntelliPed.FiveM.Messages.Puppets;
using Microsoft.Extensions.Configuration;

IConfigurationBuilder configBuilder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();

IConfigurationRoot config = configBuilder.Build();

OpenAiOptions openAiOptions = new()
{
    ApiKey = config["OpenAi:ApiKey"] ?? throw new InvalidOperationException("OpenAi:ApiKey is required"),
    OrgId = config["OpenAi:OrgId"] ?? throw new InvalidOperationException("OpenAi:ApiKey is required"),
    Model = "gpt-4o"
};

HttpClient httpClient = new();

HttpResponseMessage response = await httpClient.PostAsync("http://localhost:5000/api/puppet", null);

response.EnsureSuccessStatusCode();

CreatePuppetReply? reply = await response.Content.ReadFromJsonAsync<CreatePuppetReply>();

Agent agent = new(reply!.PedNetworkId, openAiOptions);

await agent.Think();