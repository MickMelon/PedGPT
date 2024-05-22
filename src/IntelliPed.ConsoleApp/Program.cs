﻿using IntelliPed.Core.Agents;
using IntelliPed.Core.Signals;
using Microsoft.Extensions.Configuration;

IConfigurationBuilder configBuilder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();

IConfigurationRoot config = configBuilder.Build();

OpenAiOptions openAiOptions = new()
{
    ApiKey = config["OpenAi:ApiKey"] ?? throw new InvalidOperationException("OpenAi:ApiKey is required"),
    OrgId = config["OpenAi:OrgId"] ?? throw new InvalidOperationException("OpenAi:ApiKey is required"),
    Model = "gpt-3.5-turbo-0125"
};

Agent agent = new(openAiOptions);

await agent.Start();

agent.HandleSignal(new DamageSignal
{
    DamageAmount = 1337,
    Source = "Player",
    Weapon = "Desert Eagle",
});

// Create a ManualResetEventSlim to keep the application running
ManualResetEventSlim waitHandle = new(false);
waitHandle.Wait();