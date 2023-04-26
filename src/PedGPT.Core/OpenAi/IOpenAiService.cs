namespace PedGPT.Core.OpenAi;

public interface IOpenAiService
{
    Task<Response> Completion(List<Message> messages, string model = "gpt-3.5-turbo");
}