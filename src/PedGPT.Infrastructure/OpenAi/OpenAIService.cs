﻿using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PedGPT.Infrastructure.OpenAi;

public class OpenAIService : IOpenAiService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(
        string apiKey, 
        HttpClient httpClient, 
        ILogger<OpenAIService> logger)
    {
        _apiKey = apiKey;

        _httpClient = httpClient;
        _logger = logger;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<Response> Completion(List<Message> messages, string model = "gpt-3.5-turbo")
    {
        var request = new Request(model, messages);

        _logger.LogTrace("Sending request to chat/completions.");

        var httpResponse = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", request);

        _logger.LogTrace($"Request returned {httpResponse.StatusCode} status code.");
        
        await httpResponse.Content.ReadAsStringAsync();
        httpResponse.EnsureSuccessStatusCode();
        var response = await httpResponse.Content.ReadFromJsonAsync<Response>();

        if (response is null)
            throw new Exception("OpenAI API returned null response");

        _logger.LogInformation($"Tokens used: {response.Usage.TotalTokens}");

        return response;
    }
}