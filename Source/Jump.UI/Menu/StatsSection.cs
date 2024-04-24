using Godot;
using Jump.Extensions;
using Jump.Utils;
using System;

namespace Jump.UI.Menu
{
    public class StatsSection : MenuSection
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            _progressHandler = this.GetSingleton<ProgressHandler>();
        }
        public override void Focus()
        {
            base.Focus();
            GetNode<TextureButton>("%Back").GrabFocus();
        }

        protected override void OnDisplay()
        {
            base.OnDisplay();
            PlayDisplayAnimation();
            Focus();

            UpdateLabels();
        }
        protected override void OnHide()
        {
            base.OnHide();
            PlayHideAnimation();
        }

        protected override void OnGoBack()
        {
            Hide();
            menu.DisplayExtrasSection();
        }

        private void UpdateLabels()
        {
            GetNode<Label>("%PlayTimeLabel").Text = _progressHandler.PlayTime.ToString(Constants.USER_TIME_FORMAT);
            GetNode<Label>("%AttemptsLabel").Text = _progressHandler.TotalAttempts.ToString();
            GetNode<Label>("%JumpsLabel").Text = _progressHandler.Jumps.ToString();

            GetNode<Label>("%EssenceLabel").Text = _progressHandler.Essence.ToString();


            var fragmentsCollected = _progressHandler.GlobalFragments;
            var maxFragments = _progressHandler.MaxFragments;
            GetNode<Label>("%FragmentsLabel").Text = $"{fragmentsCollected}/{maxFragments}";

            var levelsCompleted = _progressHandler.TotalLevelsComplete;
            var maxLevels = _progressHandler.MaxLevels;
            GetNode<Label>("%LevelsLabel").Text = $"{levelsCompleted}/{maxLevels}";
        }

        private ProgressHandler _progressHandler;
    }
}
