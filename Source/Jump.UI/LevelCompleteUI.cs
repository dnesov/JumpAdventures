using Godot;
using GodotFmod;
using System;

namespace Jump.UI
{
    public class LevelCompleteUI : UIElement<LevelCompleteUIData>
    {
        public override void _Process(float delta)
        {
            if (!Visible) return;
            if (Input.IsActionPressed("move_jump"))
            {
                _holdProgress += _holdProgressRate;
            }
            else
            {
                if (_loadingNextLevel) return;
                _holdProgress -= _holdProgressRate;
            }

            _holdProgress = Mathf.Clamp(_holdProgress, 0.0f, 1.0f);
            _holdEvent?.SetParameter("Progress", _holdProgress);

            if (_progressBar == null) return;
            _progressBar.Value = _holdProgress;

            if (_holdProgress == 1.0f && !_loadingNextLevel)
            {
                _loadingNextLevel = true;
                _game.LoadNextLevel();
                _holdEvent.Stop();
                _fmodRuntime.PlayOneShot("event:/UI/SectionLeave");
                return;
            }

        }
        public override void _ExitTree()
        {
            base._ExitTree();
            _holdEvent?.Stop();
        }
        protected override void OnDisplay()
        {
            Visible = true;
            Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            RectPosition = new Vector2(RectPosition.x, 200);
            PlayDisplayAnimation();
            _progressBar = GetNode<TextureProgress>("%HoldProgress");
            _game = GetTree().Root.GetNode<Game>("Game");
            _fmodRuntime = GetTree().Root.GetNode<FmodRuntime>("FmodRuntime");
            _holdEvent = _fmodRuntime.GetEventInstance("event:/UI/Hold");
            _holdEvent.Start();

            var levelCompleteHint = GetNode<RichTextLabel>("%LevelCompleteHint");
            levelCompleteHint.BbcodeText = $"[center]{Tr("UI_LEVELCOMPLETE_HINT")}";
        }

        private void PlayDisplayAnimation()
        {
            var tween = CreateTween();
            tween.Parallel().TweenProperty(this, "modulate", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.55f);
            tween.Parallel().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out).TweenProperty(this, "rect_position", new Vector2(0, 0), 0.35f);
        }

        protected override void OnHide()
        {

        }

        protected override void OnUpdateElements(LevelCompleteUIData data)
        {
            GetNode<Label>("%AttemptsValue").Text = data.Attempts.ToString();
            GetNode<Label>("%EssenceValue").Text = data.EssenceCollected.ToString();
        }

        [Export] private float _holdProgressRate = 0.02f;
        private float _holdProgress = 0f;
        private TextureProgress _progressBar;
        private Game _game;
        private bool _loadingNextLevel;

        private FmodRuntime _fmodRuntime;
        private FmodEventInstance _holdEvent;
    }

    public class LevelCompleteUIData : UIData
    {
        public int Attempts;
        public int EssenceCollected;
        public float TimeInSecondsSinceStart;
    }
}
