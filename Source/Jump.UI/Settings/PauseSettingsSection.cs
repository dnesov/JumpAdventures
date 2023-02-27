using Godot;
using Jump.UI.Menu;
using Jump.UI.Settings;
using System;
using System.Collections.Generic;

namespace Jump.UI.Settings
{
    public class PauseSettingsSection : Section
    {
        public override void _Ready()
        {
            base._Ready();
            GetNode<ButtonMinimal>("%Back").OnPressedAction += BackPressed;
            _game = GetTree().Root.GetNode<Game>("Game");
        }
        protected override void OnDisplay()
        {
            PlayDisplayAnimation();
            UpdateProperties();
        }
        protected override void OnHide() => PlayHideAnimation();

        private void UpdateProperties()
        {
            var subsections = GetNode<VBoxContainer>("%Subsections");
            foreach (SettingsSubsection subsection in subsections.GetChildren())
            {
                subsection.UpdateElements(_game.DataHandler.Data);
            }
        }

        private void BackPressed()
        {
            _game.DataHandler.SaveData();

            var continueButton = GetOwner<PauseMenu>().GetNode<ButtonMinimal>("%Continue");
            continueButton.GrabFocus();

            Hide();
        }

        private Game _game;
    }
}