using Godot;
using Jump.Extensions;
using System;

namespace Jump.UI
{
    public class CollectiblesUI : UIElement
    {
        public override void _Ready()
        {
            base._Ready();
            _game = this.GetSingleton<Game>();
            _essenceContainer = GetNode<HBoxContainer>("%EssenceContainer");

            _game.CurrentGameMode.OnEssenceCollected += EssenceCollected;
            _game.CurrentGameMode.OnEssenceChanged += OnUpdateElements;

            _game.OnWin += OnHide;
            _game.OnRetry += OnDisplay;

            _timerLabel = GetNode<Label>("%TimerLabel");

            var timerContainer = GetNode<HBoxContainer>("%TimerContainer");
            timerContainer.Visible = _game.UseTimer;
        }
        public override void _ExitTree()
        {
            base._ExitTree();

            _game.CurrentGameMode.OnEssenceCollected -= EssenceCollected;
            _game.CurrentGameMode.OnEssenceChanged -= OnUpdateElements;
            _game.OnWin -= OnHide;
            _game.OnRetry -= OnDisplay;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            _timerLabel.Text = _game.TimerFormatted;
        }


        protected override void OnDisplay()
        {
            Visible = true;
        }

        protected override void OnHide()
        {
            Visible = false;
        }

        protected override void OnUpdateElements()
        {
            GetNode<Label>("%EssenceLabel").Text = $"{_game.CurrentGameMode.Essence}";
        }

        private void EssenceCollected()
        {
            _essenceContainer.RectPivotOffset = _essenceContainer.RectSize / 2f;

            var essenceParticles = _essenceContainer.GetNode<Particles2D>("EssenceParticles");
            essenceParticles.Position = _essenceContainer.RectSize / 2f;
            essenceParticles.Restart();

            var tween = _essenceContainer.CreateTween();

            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(_essenceContainer, "rect_scale", new Vector2(1.2f, 1.2f), 0.05f);
            tween.Chain().TweenProperty(_essenceContainer, "rect_scale", Vector2.One, 0.15f);
        }

        private HBoxContainer _essenceContainer;
        private Game _game;

        private Label _timerLabel;
    }
}