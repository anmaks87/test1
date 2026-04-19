using System.Net.Http.Json;

namespace TelegramSenderDesktop;

public sealed class TelegramService
{
    private readonly HttpClient _httpClient;

    public TelegramService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendMessageAsync(string botToken, string chatId, string text, CancellationToken cancellationToken)
    {
        var endpoint = $"https://api.telegram.org/bot{botToken}/sendMessage";
        using var response = await _httpClient.PostAsJsonAsync(
            endpoint,
            new
            {
                chat_id = chatId,
                text
            },
            cancellationToken);

        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Telegram API error: {responseText}");
        }
    }
}
