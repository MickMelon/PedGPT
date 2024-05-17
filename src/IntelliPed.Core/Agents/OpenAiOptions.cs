using System.ComponentModel.DataAnnotations;

namespace IntelliPed.Core.Agents;

public record OpenAiOptions
{
    [Required]
    public string ApiKey { get; init; } = "";

    [Required]
    public string OrgId { get; init; } = "";

    [Required]
    public string Model { get; init; } = "";
}