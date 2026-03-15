using System.Text.Json;

namespace TeamsHider;

public class Config
{
    public bool HideTopBar { get; set; } = true;
    public bool HideBottomOverlay { get; set; } = true;

    private static Config? _instance;

    public static Config Instance => _instance ??= Load();

    private static Config Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "settings.json");
        if (!File.Exists(path)) return new Config();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Config>(json) ?? new Config();
    }
}
