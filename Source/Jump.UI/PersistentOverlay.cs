using Godot;
using Jump.Extensions;
using System;

namespace Jump.UI
{
    public class PersistentOverlay : CanvasLayer
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _versionLabel = GetNode<Label>("%Version");
            _versionLabel.Text = $"{Constants.VERSION_MAJOR}.{Constants.VERSION_MINOR}.{Constants.VERSION_PATCH}-{Constants.BUILD_TYPE}";

            _fpsLabel = GetNode<Label>("%FPS");
            GetNode<Label>("%DebugBuild").Visible = Constants.IsDebugBuild;

            var game = this.GetSingleton<Game>();
            game.OnHideUi += delegate () { Visible = false; };
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            _fpsLabel.Text = $"{Engine.GetFramesPerSecond()} FPS";
        }

        private Label _versionLabel;
        private Label _fpsLabel;
    }
}
