using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using PedGPT.Core;
using PedGPT.Core.OpenAi;
using Xunit;

namespace PedGPT.Tests;

public class AgentTests
{
    [Fact]
    public async Task ShouldParseOpenAiResponse()
    {
        var openAiResponseJson = await File.ReadAllTextAsync("TestData/openai-response.json");

        var openAiResponse = JsonSerializer.Deserialize<Response>(openAiResponseJson);

        var openAiServiceMock = new Mock<IOpenAiService>();

        openAiServiceMock
            .Setup(_ => _.Completion(It.IsAny<List<Message>>(), It.IsAny<string>()))
            .ReturnsAsync(openAiResponse!);

        var agent = new Agent(
            new Memory(), 
            openAiServiceMock.Object,
            Mock.Of<ILogger<Agent>>());

        var thoughts = await agent.Think(new());

        thoughts.Should().NotBeNull();

        using (new AssertionScope())
        {
            thoughts.Thoughts.Text.Should().Be("I need to drive to the police station. Getting there quickly will be important to avoid delays and potential issues with law enforcement. I will need to stay alert and cautious while en route to ensure I arrive safely.");
            thoughts.Thoughts.Reasoning.Should().Be("Driving to the police station will require me to follow the road rules, use defensive driving techniques, and avoid any obstacles or hazards that might be present. Staying focused and aware of my surroundings will be key to completing the task successfully.");
            thoughts.Thoughts.Plan.Should().Be("- Start driving towards the police station.\n- Maintain a safe speed and obey all traffic signals and signs.\n- Keep an eye out for any potential hazards or obstacles and navigate around them as necessary.");
            thoughts.Thoughts.Criticism.Should().Be("I could have considered taking a different vehicle, such as a faster sports car, to get there faster. Additionally, I should always be on the lookout for potential threats while driving.");
            thoughts.Thoughts.Speak.Should().Be("I am driving to the police station now. I will obey traffic laws and stay focused on the road to ensure a safe arrival.");

            thoughts.Command!.Name.Should().Be("drive_to");
            thoughts.Command.Args.Should().Contain("destination", "police station");
        }
    }
}