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
    public const short VERSION_PATCH = 2;
    public const string BUILD_TYPE = "release";
    public static readonly bool IsDebugBuild = OS.HasFeature("debug");
    public static readonly bool IsSteamBuild = OS.HasFeature("Steam");

    public static string VersionFull => $"{VERSION_MAJOR}.{VERSION_MINOR}.{VERSION_PATCH}-{BUILD_TYPE}";
    public static readonly string TimerFormat = "{0:0.00}";
    public const string USER_TIME_FORMAT = @"hh\:mm\:ss";
    public const string LOG_TIME_FORMAT = @"hh\:mm\:ss\:fff";
    // public static string FeedbackFormLink => "https://docs.google.com/forms/d/e/1FAIpQLScJalwx_tuj8ZznQC8kSPCDDzsnjx6YMgdFR8B0x-qrOpIPeQ/viewform?usp=pp_url&entry.731835764=GAME_VERSION&entry.221682950=LEVEL_NAME";

    // public static void OpenFeedbackForm(string currentLevelString = "")
    // {
    //     var url = Constants.FeedbackFormLink;
    //     url = url.Replace("GAME_VERSION", Constants.VersionFull);
    //     url = url.Replace("LEVEL_NAME", currentLevelString);
    //     OS.ShellOpen(url);
    // }

    public static readonly Dictionary<string, string> SupportedLanguages = new Dictionary<string, string>()
        {
            {"en", "English"},
            {"pl", "Polski"},
            {"uk", "Українська"},
            {"ru", "Русский"},
            // {"nl", "Dutch"}
        };

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