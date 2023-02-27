using Godot;
using Jump.Utils;

public class Main : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _logger.Info($"Jump Adventures ({Constants.VersionFull})");
        var sceneSwitcher = GetTree().Root.GetNode<SceneSwitcher>("SceneSwitcher");

        var game = GetTree().Root.GetNode<Game>("Game");

        sceneSwitcher.Load(game.DataHandler.Data.UseNewMenu ? Constants.NEW_MAIN_MENU_PATH : Constants.OLD_MAIN_MENU_PATH);
    }

    private Logger _logger = new Logger(nameof(Main));
}
