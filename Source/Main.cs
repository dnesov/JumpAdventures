using Godot;
using Jump.Extensions;
using Jump.Utils;

public class Main : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _logger.Info($"Jump Adventures ({Constants.VersionFull})");

        var sceneSwitcher = this.GetSingleton<SceneSwitcher>();
        var game = this.GetSingleton<Game>();

        sceneSwitcher.LoadWithSplash(Constants.NEW_MAIN_MENU_PATH);
    }

    private Logger _logger = new Logger(nameof(Main));
}
