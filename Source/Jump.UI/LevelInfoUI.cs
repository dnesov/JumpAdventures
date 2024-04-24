using Godot;
using Jump.Extensions;
using System;

namespace Jump.UI
{
    public class LevelInfoUI : MarginContainer
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GetNodes();
            SubscribeEvents();
            UpdateLabels();
        }

        public override void _ExitTree()
        {
            UnsubscribeEvents();
        }

        public override void _Notification(int what)
        {
            if (what == NotificationTranslationChanged)
            {
                UpdateLabels();
            }
        }

        private void SubscribeEvents()
        {
            _game.OnPaused += OnPaused;
            _game.OnResumed += OnResumed;
        }

        private void UnsubscribeEvents()
        {
            _game.OnPaused -= OnPaused;
            _game.OnResumed -= OnResumed;
        }

        private void GetNodes()
        {
            _game = this.GetSingleton<Game>();

            _worldNameLabel = GetNode<Label>("VBoxContainer/World");
            _levelNameLabel = GetNode<Label>("VBoxContainer/Level");
        }

        private void UpdateLabels()
        {
            // TODO: i dont like this.
            var levelName = string.Format(Tr(_game.CurrentLevel.Name), _game.CurrentWorld.GetLevelIdx(_game.CurrentLevel) + 1);
            _worldNameLabel.Text = _game.CurrentWorld.Name;
            _levelNameLabel.Text = levelName;
        }

        private void OnPaused() => Visible = false;
        private void OnResumed() => Visible = true;

        private Label _worldNameLabel, _levelNameLabel;
        private Game _game;
    }
}