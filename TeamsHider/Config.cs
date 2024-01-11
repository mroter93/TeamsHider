namespace TeamsHider;

using JsonConfig;

public static class Config
{
    public static bool HideTopBar
    {
        get => (bool)JsonConfig.Config.Global.HideTopBar;
        set => JsonConfig.Config.Global.HideTopBar = value;
    }
    
    public static bool HideBottomOverlay
    {
        get => (bool)JsonConfig.Config.Global.HideBottomOverlay;
        set => JsonConfig.Config.Global.HideBottomOverlay = value;
    }
    
    public static bool GetWindows
    {
        get => (bool)JsonConfig.Config.Global.GetWindows;
        set => JsonConfig.Config.Global.GetWindows = value;
    }
    
    public static string TopBarWindowName
    {
        get => (string)JsonConfig.Config.Global.TopBarWindowName;
        set => JsonConfig.Config.Global.TopBarWindowName = value;
    }
}