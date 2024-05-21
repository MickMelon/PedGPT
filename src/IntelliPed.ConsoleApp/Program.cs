using System.Net.Http.Json;
using IntelliPed.Core.Agents;
using IntelliPed.Core.Signals;
using IntelliPed.FiveM.Messages.Puppets;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

await SignalR();

// Create a ManualResetEventSlim to keep the application running
ManualResetEventSlim waitHandle = new(false);
waitHandle.Wait();

IConfigurationBuilder configBuilder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();

IConfigurationRoot config = configBuilder.Build();

OpenAiOptions openAiOptions = new()
{
    ApiKey = config["OpenAi:ApiKey"] ?? throw new InvalidOperationException("OpenAi:ApiKey is required"),
    OrgId = config["OpenAi:OrgId"] ?? throw new InvalidOperationException("OpenAi:ApiKey is required"),
    Model = "gpt-3.5-turbo-0125"
};
HttpClient httpClient = new();

HttpResponseMessage response = await httpClient.PostAsync("http://localhost:5000/api/puppet", null);

response.EnsureSuccessStatusCode();

CreatePuppetReply? reply = await response.Content.ReadFromJsonAsync<CreatePuppetReply>();

Agent agent = new(reply!.PedNetworkId, openAiOptions);

agent.SignalProcessor.Start();
agent.SignalProcessor.HandleSignal(new DamageSignal
{
    DamageAmount = 1337, 
    Source = "Player",
    Weapon = "Desert Eagle",
});



static async Task SignalR()
{
    // Create a connection to the SignalR hub
    var connection = new HubConnectionBuilder()
        .WithUrl("http://localhost:5000/my-hub")
        .Build();

    // Register a handler for messages from the hub
    connection.On<string, string>("ReceiveMessage", (user, message) =>
    {
        Console.WriteLine($"{user}: {message}");
    });

    try
    {
        // Start the connection
        await connection.StartAsync();
        Console.WriteLine("Connection started");

        // Send a message to the hub
        await connection.InvokeAsync("SendMessage", "ConsoleClient", "Hello from the console app!");

        // Keep the console open
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}