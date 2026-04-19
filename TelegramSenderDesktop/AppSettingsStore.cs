using System.Text.Json;

namespace TelegramSenderDesktop;

public sealed class AppSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _settingsPath;

    public AppSettingsStore(string settingsPath)
    {
        _settingsPath = settingsPath;
    }

    public AppSettings Load()
    {
        EnsureDirectory();

        if (!File.Exists(_settingsPath))
        {
            var defaults = CreateDefaultSettings();
            Save(defaults);
            return defaults;
        }

        var json = File.ReadAllText(_settingsPath);
        var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);

        if (settings is null)
        {
            var defaults = CreateDefaultSettings();
            Save(defaults);
            return defaults;
        }

        settings.Recipients ??= [];
        return settings;
    }

    public void Save(AppSettings settings)
    {
        EnsureDirectory();
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(_settingsPath, json);
    }

    private void EnsureDirectory()
    {
        var directory = Path.GetDirectoryName(_settingsPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private static AppSettings CreateDefaultSettings()
    {
        return new AppSettings
        {
            Recipients =
            [
                new Recipient { Name = "Даня", ChatId = "" },
                new Recipient { Name = "Мама", ChatId = "" },
                new Recipient { Name = "Тато", ChatId = "" }
            ]
        };
    }
}
