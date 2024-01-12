namespace TeamsHider;

using JsonConfig;

public static class Config
{
    public static bool HideTopBar => (bool)JsonConfig.Config.Global.HideTopBar;

    public static bool HideBottomOverlay => (bool)JsonConfig.Config.Global.HideBottomOverlay;
}