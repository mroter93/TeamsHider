using System.Text.Json;

namespace TeamsHider;

public class Config
{
    public bool HideTopBar { get; set; } = true;
    public bool HideBottomOverlay { get; set; } = true;

    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "settings.json");
    private static Config _instance = Load();
    private static FileSystemWatcher? _watcher;

    public static Config Instance => _instance;

    public static void Watch()
    {
        _watcher = new FileSystemWatcher(Path.GetDirectoryName(FilePath)!, Path.GetFileName(FilePath))
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
            EnableRaisingEvents = true
        };
        _watcher.Changed += (_, _) => Reload();
    }

    private static void Reload()
    {
        try
        {
            _instance = Load();
        }
        catch
        {
            // keep current config if file is invalid
        }
    }

    private static Config Load()
    {
        if (!File.Exists(FilePath)) return new Config();
        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<Config>(json) ?? new Config();
    }
}
