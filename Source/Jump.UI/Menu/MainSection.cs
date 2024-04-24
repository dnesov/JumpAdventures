using Godot;
using Jump.Extensions;
using System;

namespace Jump.UI.Menu
{
    public class MainSection : MenuSection
    {
        public override void _Ready()
        {
            base._Ready();
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GetNode<ButtonMinimal>("%Play").OnPressedAction += PlayPressed;
            GetNode<ButtonMinimal>("%Settings").OnPressedAction += SettingsPressed;
            GetNode<ButtonMinimal>("%Customize").OnPressedAction += CustomizePressed;
            GetNode<ButtonMinimal>("%Extras").OnPressedAction += ExtrasPressed;
            GetNode<ButtonMinimal>("%Exit").OnPressedAction += Exit;
        }

        private void UnsubscribeEvents()
        {
            GetNode<ButtonMinimal>("%Play").OnPressedAction -= PlayPressed;
            GetNode<ButtonMinimal>("%Settings").OnPressedAction -= SettingsPressed;
            GetNode<ButtonMinimal>("%Customize").OnPressedAction -= CustomizePressed;
            GetNode<ButtonMinimal>("%Extras").OnPressedAction -= ExtrasPressed;
            GetNode<ButtonMinimal>("%Exit").OnPressedAction -= Exit;
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

        private void CustomizePressed()
        {
            Hide();
            menu.DisplayCustomizeSection();
        }

        private void ExtrasPressed()
        {
            Hide();
            menu.DisplayExtrasSection();
        }

        private void UpdateHint(string hint) => GetNode<Label>("%Hint").Text = hint;

        private void Exit()
        {
            var game = this.GetSingleton<Game>();
            game.Quit();
        }

        protected override void OnDisplay()
        {
            base.OnDisplay();
            SubscribeEvents();
            PlayDisplayAnimation();
            Focus();

            soundtrackManager.PlayTrack(_mainMenuMusicPath);
        }
        protected override void OnHide()
        {
            base.OnHide();
            PlayHideAnimation();
            UnsubscribeEvents();
        }

        public override void Focus()
        {
            base.Focus();
            GetNode<ButtonMinimal>("%Play").GrabFocus();
        }
        protected override void OnGoBack()
        {
            return;
        }

        private readonly string _mainMenuMusicPath = "event:/Soundtrack/Menu";
    }
}
