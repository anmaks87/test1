using System.Text.Json.Serialization;

namespace TelegramSenderDesktop;

public sealed class AppSettings
{
    [JsonPropertyName("botToken")]
    public string BotToken { get; set; } = string.Empty;

    [JsonPropertyName("defaultMessage")]
    public string DefaultMessage { get; set; } = "Привет";

    [JsonPropertyName("testMessage")]
    public string TestMessage { get; set; } = "Привет";

    [JsonPropertyName("recipients")]
    public List<Recipient> Recipients { get; set; } = [];
}

public sealed class Recipient
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("chatId")]
    public string ChatId { get; set; } = string.Empty;

    public override string ToString() => Name;
}
