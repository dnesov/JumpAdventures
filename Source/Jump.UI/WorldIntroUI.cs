using Godot;
using System;

namespace Jump.UI
{
    public class WorldIntroUI : UIElement<WorldIntroData>
    {
        public override void _Ready()
        {
            base._Ready();
            _game = GetTree().Root.GetNode<Game>("Game");
        }
        private void PlayDisplayAnimation()
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("show", default, _animationTimeScale);
        }
        protected override void OnDisplay() => PlayDisplayAnimation();

        protected override void OnHide()
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateElements(WorldIntroData data)
        {
            // TODO: why
            var levelName = String.Format(Tr(_game.CurrentLevel.Name), _game.CurrentWorld.GetLevelIdx(_game.CurrentLevel) + 1);

            GetNode<Label>("%WorldName").Text = data.WorldName;
            GetNode<Label>("%LevelName").Text = levelName;

            _animationTimeScale = data.DisplayAnimTimeScale;
        }

        private float _animationTimeScale;
        private Game _game;
    }

    public class WorldIntroData
    {
        public float DisplayAnimTimeScale = 1.0f;
        public string WorldName;
        public string LevelName;
    }
}
