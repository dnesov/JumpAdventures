using Godot;
using Jump.Levels;
using System;
using System.Collections.Generic;

namespace Jump.UI.Menu
{
    public class LevelList : UIElement<List<LevelButtonData>>
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _levelContainer = GetNode<VBoxContainer>("%LevelContainer");
        }

        protected override void OnDisplay()
        {
            Visible = true;
        }

        protected override void OnHide()
        {
            Visible = false;
        }

        protected override void OnUpdateElements(List<LevelButtonData> data)
        {
            ClearLevelButtons();

            short i = 0;
            foreach (var level in data)
            {
                _levelContainer.AddChild(CreateLevelButton(i, level));
                i++;
            }
        }

        private void ClearLevelButtons()
        {
            foreach (LevelButton child in _levelContainer.GetChildren())
                child.QueueFree();
        }

        private LevelButton CreateLevelButton(short idx, LevelButtonData level)
        {
            var levelButton = _levelButtonScene.Instance<LevelButton>();
            levelButton.LevelId = idx;
            levelButton.UpdateElements(level);
            levelButton.OnPressed += OnLevelButtonPressed;
            return levelButton;
        }

        private void OnLevelButtonPressed(short id)
        {
            var game = GetTree().Root.GetNode<Game>("Game");
            var level = game.CurrentWorld.Levels[id];
            game.LoadLevel(level);
        }

        [Export] private PackedScene _levelButtonScene;
        private VBoxContainer _levelContainer;
    }
}