using Godot;

public static class Constants
{
    public const string OLD_MAIN_MENU_PATH = "res://Menu/Menu.tscn";
    public const string NEW_MAIN_MENU_PATH = "res://Prefabs/UI/MainMenuNew/MenuNew.tscn";
    public const string CREDITS_SCENE_PATH = "res://Menu/Views/Credits.tscn";
    public const long DISCORD_CLIENT_ID = 898238428603383828;
    public const uint STEAM_APP_ID = 2221130;
    public const short VERSION_MAJOR = 1;
    public const short VERSION_MINOR = 0;
    public const short VERSION_PATCH = 0;
    public const string BUILD_TYPE = "alpha5";
    public static readonly bool IsDebugBuild = OS.HasFeature("debug");

    public static string VersionFull => $"{VERSION_MAJOR}.{VERSION_MINOR}.{VERSION_PATCH}-{BUILD_TYPE}";
}