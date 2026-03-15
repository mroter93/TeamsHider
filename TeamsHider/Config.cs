using System.Text.Json;

namespace TeamsHider;

public class Config
{
    public bool HideTopBar { get; set; } = true;
    public bool HideBottomOverlay { get; set; } = true;

    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "settings.json");
    private static Config _instance = Load();
    private static DateTime _lastModified;

    public static Config Instance => _instance;

    public static void ReloadIfChanged()
    {
        try
        {
            if (!File.Exists(FilePath)) return;
            var modified = File.GetLastWriteTimeUtc(FilePath);
            if (modified == _lastModified) return;
            _lastModified = modified;
            _instance = Load();
        }
        catch
        {
            // keep current config if file is unreadable
        }
    }

    private static Config Load()
    {
        if (!File.Exists(FilePath)) return new Config();
        _lastModified = File.GetLastWriteTimeUtc(FilePath);
        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<Config>(json) ?? new Config();
    }
}
