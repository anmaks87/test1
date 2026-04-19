namespace TelegramSenderDesktop;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
        var settingsPath = Path.Combine(dataDirectory, "settings.json");

        using var httpClient = new HttpClient();
        var settingsStore = new AppSettingsStore(settingsPath);
        var telegramService = new TelegramService(httpClient);

        Application.Run(new MainForm(settingsStore, telegramService));
    }
}
