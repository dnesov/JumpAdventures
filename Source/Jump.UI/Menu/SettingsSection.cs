using Godot;
using Jump.Extensions;
using Jump.UI.Settings;
using System;
using System.Collections.Generic;

namespace Jump.UI.Menu
{
    public class SettingsSection : MenuSection
    {
        [Export] public NodePath FocusOn { get; set; }
        public override void _Ready()
        {
            base._Ready();
            _game = this.GetSingleton<Game>();
        }

        public override void Focus()
        {
            GetNode<Control>(FocusOn).GrabFocus();
        }

        protected override void OnDisplay()
        {
            base.OnDisplay();
            PlayDisplayAnimation();
            UpdateProperties();
            Focus();
        }
        protected override void OnHide()
        {
            base.OnHide();
            PlayHideAnimation();
        }

        private void UpdateProperties()
        {
            var subsections = GetNode<VBoxContainer>("%Subsections");
            foreach (SettingsSubsection subsection in subsections.GetChildren())
            {
                subsection.UpdateElements(_game.DataHandler.Data);
            }
        }

        protected override void OnGoBack()
        {
            _game.DataHandler.SaveData();
            Hide();

            if (menu != null)
            {
                menu.DisplayMainSection();
            }
        }

        private Game _game;
    }
}