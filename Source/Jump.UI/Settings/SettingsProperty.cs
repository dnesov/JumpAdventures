using System;
using Godot;

namespace Jump.UI.Settings
{
    public abstract class SettingsProperty : UIElement<GlobalData>
    {
        public override void _Ready()
        {
            base._Ready();
            var game = GetTree().Root.GetNode<Game>("Game");

            data = game.DataHandler.Data;
        }

        public abstract bool IsDisabled();

        protected GlobalData data;
    }
}