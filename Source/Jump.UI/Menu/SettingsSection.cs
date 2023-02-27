using Godot;
using Jump.UI.Settings;
using System;
using System.Collections.Generic;

namespace Jump.UI.Menu
{
    public class SettingsSection : MenuSection
    {
        public override void _Ready()
        {
            base._Ready();
            GetNode<ButtonMinimal>("%Back").OnPressedAction += GoBack;
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

        protected override void GoBack()
        {
            _game.DataHandler.SaveData();
            Hide();
            menu.ReturnToMainSection();
        }

        private Game _game;
    }
}