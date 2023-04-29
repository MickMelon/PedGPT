using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PedGPT.Core.Json;

namespace PedGPT.Core.OpenAi;

public class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAiService> _logger;
    private readonly IJsonSerializer _jsonSerializer;

    public OpenAiService(
        string apiKey, 
        HttpClient httpClient, 
        ILogger<OpenAiService> logger,
        IJsonSerializer jsonSerializer)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonSerializer = jsonSerializer;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<Response> Completion(List<Message> messages, string model = "gpt-3.5-turbo")
    {
        var request = new Request(model, messages);

        _logger.LogTrace("Sending request to chat/completions.");

        var httpResponse = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", request);

        _logger.LogTrace($"Request returned {httpResponse.StatusCode} status code.");
        
        var contentString = await httpResponse.Content.ReadAsStringAsync();

        _logger.LogTrace("Response content: {responseContent}", contentString);

        httpResponse.EnsureSuccessStatusCode();

        var fixedJson = JsonFixer.FixJson(contentString);

        var response = _jsonSerializer.Deserialize<Response>(fixedJson);

        _logger.LogInformation($"Tokens used: {response!.Usage.TotalTokens}");

        return response;
    }
}