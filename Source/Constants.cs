using System.Collections.Generic;
using Godot;

public static class Constants
{
    public const string NEW_MAIN_MENU_PATH = "res://Prefabs/UI/MainMenuNew/MenuNew.tscn";
    public const string LEVEL_EDITOR_PATH = "res://Prefabs/UI/Editor/Editor.tscn";
    public const string CREDITS_SCENE_PATH = "res://Menu/Views/Credits.tscn";
    public const long DISCORD_CLIENT_ID = 898238428603383828;
    public const uint STEAM_APP_ID = 2221130;
    public const short VERSION_MAJOR = 1;
    public const short VERSION_MINOR = 0;
    public const short VERSION_PATCH = 0;
    public const string BUILD_TYPE = "stable";
    public static readonly bool IsDebugBuild = OS.HasFeature("debug");
    public static readonly bool IsSteamBuild = OS.HasFeature("Steam");

    public static string VersionFull => $"{VERSION_MAJOR}.{VERSION_MINOR}.{VERSION_PATCH}-{BUILD_TYPE}";
    public static readonly string TimerFormat = "{0:0.00}";
    public const string USER_TIME_FORMAT = @"hh\:mm\:ss";
    public const string LOG_TIME_FORMAT = @"hh\:mm\:ss\:fff";

    public static class WorldIds
    {
        public static readonly string Seaside = "ja_world1_seaside";
        public static readonly string Forests = "ja_world2_forests";
        public static readonly string LavenderFields = "ja_world3_lavenderfields";

        public static string[] AsArray = new string[]
        {
            Seaside,
            Forests,
            LavenderFields
        };

        public static readonly Dictionary<string, string> ImagePaths = new()
        {
            {Seaside, "res://Assets/MenuBackgrounds/seaside.jpg"},
            {Forests, "res://Assets/MenuBackgrounds/forests.jpg"},
            {LavenderFields, "res://Assets/MenuBackgrounds/lavender.jpg"},
        };
    }
}