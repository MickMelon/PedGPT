using System.Text;

namespace IntelliPed.Core.Agents;

public record PersonalInfo
{
    public required string Name { get; init; }
    public required int Age { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Address { get; init; }
    public List<string>? PersonalityTraits { get; init; }
    public List<string>? Interests { get; init; }
    public string? Occupation { get; init; }
    public string? Education { get; init; }
    public List<string>? Goals { get; init; }
    public List<string>? Skills { get; init; }
    public Dictionary<string, string>? Preferences { get; init; } // e.g., {"FavoriteFood": "Pizza"}
    public string? Biography { get; init; }
    public List<string>? Family { get; init; } // e.g., {"Father: John Doe", "Mother: Jane Doe"}
    public List<string>? SignificantLifeEvents { get; init; } // e.g., {"Graduated college in 2010"}
    public List<string>? SocialCircle { get; init; } // e.g., {"Friend: Alice", "Mentor: Bob"}
    public string? CommunicationStyle { get; init; }
    public string? EmotionalState { get; init; }
    public List<string>? Values { get; init; } // e.g., {"Honesty", "Loyalty"}

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.AppendLine($"Name: {Name}");
        builder.AppendLine($"Age: {Age}");
        if (DateOfBirth.HasValue) builder.AppendLine($"Date of Birth: {DateOfBirth:MMMM dd, yyyy}");
        if (!string.IsNullOrWhiteSpace(Address)) builder.AppendLine($"Address: {Address}");
        if (PersonalityTraits != null && PersonalityTraits.Any()) builder.AppendLine($"Personality Traits: {string.Join(", ", PersonalityTraits)}");
        if (Interests != null && Interests.Any()) builder.AppendLine($"Interests: {string.Join(", ", Interests)}");
        if (!string.IsNullOrWhiteSpace(Occupation)) builder.AppendLine($"Occupation: {Occupation}");
        if (!string.IsNullOrWhiteSpace(Education)) builder.AppendLine($"Education: {Education}");
        if (Goals != null && Goals.Any()) builder.AppendLine($"Goals: {string.Join(", ", Goals)}");
        if (Skills != null && Skills.Any()) builder.AppendLine($"Skills: {string.Join(", ", Skills)}");
        if (Preferences != null && Preferences.Any()) builder.AppendLine($"Preferences: {string.Join(", ", Preferences.Select(kv => $"{kv.Key}: {kv.Value}"))}");
        if (!string.IsNullOrWhiteSpace(Biography)) builder.AppendLine($"Biography: {Biography}");
        if (Family != null && Family.Any()) builder.AppendLine($"Family: {string.Join(", ", Family)}");
        if (SignificantLifeEvents != null && SignificantLifeEvents.Any()) builder.AppendLine($"Significant Life Events: {string.Join(", ", SignificantLifeEvents)}");
        if (SocialCircle != null && SocialCircle.Any()) builder.AppendLine($"Social Circle: {string.Join(", ", SocialCircle)}");
        if (!string.IsNullOrWhiteSpace(CommunicationStyle)) builder.AppendLine($"Communication Style: {CommunicationStyle}");
        if (!string.IsNullOrWhiteSpace(EmotionalState)) builder.AppendLine($"Emotional State: {EmotionalState}");
        if (Values != null && Values.Any()) builder.AppendLine($"Values: {string.Join(", ", Values)}");

        return builder.ToString();
    }
}