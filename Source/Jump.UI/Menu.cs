using Godot;
using Jump.Extensions;
using Jump.Utils;
using System;

public class Menu : MarginContainer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();

        // Focus the play button on load.
        var dh = this.GetSingleton<Game>().DataHandler;
        if (!dh.Data.AcceptedNotice) return;
        GetNode<TextureButton>("%Play").GrabFocus();
    }

    public void SetHint(string hintString)
    {
        _hintLabel.Text = hintString;
    }

    public void OpenCredits()
    {
        var switcher = this.GetSingleton<SceneSwitcher>();
        switcher.Load(Constants.CREDITS_SCENE_PATH, true, 1.5f);
        _logger.Info("Opening credits.");
    }

    private void GetNodes()
    {
        _hintLabel = GetNode<Label>(_hintLabelPath);
    }

    private void PlayButtonPressed()
    {
        _logger.Info("Playing the game!");
        var switcher = this.GetSingleton<SceneSwitcher>();
        switcher.Load("res://Menu/Views/LevelSelectMenu.tscn", true, 1.5f);
    }

    private void ExitGamePressed()
    {
        var game = this.GetSingleton<Game>();
        game.Quit();
    }

    private Logger _logger = new Logger(nameof(Menu));
    [Export(PropertyHint.File, "*.tscn")] private string _creditsScenePath;

    [Export] private NodePath _hintLabelPath;
    private Label _hintLabel;
}