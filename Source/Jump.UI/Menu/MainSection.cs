using Godot;
using System;

namespace Jump.UI.Menu
{
    public class MainSection : MenuSection
    {
        public override void _Ready()
        {
            base._Ready();
            ConnectSignals();
        }

        private void ConnectSignals()
        {
            GetNode<ButtonMinimal>("%Play").OnPressedAction += PlayPressed;
            GetNode<ButtonMinimal>("%Settings").OnPressedAction += SettingsPressed;
            GetNode<ButtonMinimal>("%Extras").OnPressedAction += ExtrasPressed;
            GetNode<ButtonMinimal>("%Exit").OnPressedAction += Exit;
        }

        private void SettingsPressed()
        {
            Hide();
            menu.DisplaySettingsSection();
        }
        private void PlayPressed()
        {
            Hide();
            menu.DisplayPlaySection();
        }

        private void ExtrasPressed()
        {
            Hide();
            menu.DisplayExtrasSection();
        }

        private void UpdateHint(string hint) => GetNode<Label>("%Hint").Text = hint;

        private void Exit()
        {
            var game = GetTree().Root.GetNode<Game>("Game");
            game.Quit();
        }

        protected override void OnDisplay()
        {
            PlayDisplayAnimation();
            Focus();
        }
        protected override void OnHide() => PlayHideAnimation();

        public override void Focus()
        {
            base.Focus();
            GetNode<ButtonMinimal>("%Play").GrabFocus();
        }
        protected override void GoBack()
        {
            return;
        }
    }
}
