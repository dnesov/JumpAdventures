using Godot;
using Jump.Extensions;
using Jump.Misc;
using System;

namespace Jump.Entities
{
    public class Essence : Collectible
    {
        public override void _Ready()
        {
            base._Ready();
            _game = this.GetSingleton<Game>();
            SubscribeEvents();

            if (_game.CurrentGameMode is ChallengeGameMode)
            {
                GetNode<Sprite>("Sprite").Modulate = _challengeModeColor;
            }

            root.MaxEssence += 1;
        }
        // Called when the node enters the scene tree for the first time.
        public override void _PhysicsProcess(float delta)
        {
            if (_collected) return;
            _timer += delta;
            Animate(delta);
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _game.OnLateRetry += Respawn;
        }

        private void UnsubscribeEvents()
        {
            _game.OnLateRetry -= Respawn;
        }

        private void Respawn()
        {
            if (_tween != null && _tween.IsRunning())
            {
                _tween.Stop();
            }

            _collected = false;

            Scale = Vector2.One;
            Visible = true;
            Modulate = new Color(1f, 1f, 1f, 1f);
        }

        private void Animate(float delta)
        {
            if (_collected) return;
            Position = Position.LinearInterpolate(new Vector2(Position.x, Position.y + Mathf.Sin(_timer * 2.0f) * 10), delta);
            Rotation = Mathf.LerpAngle(Rotation, Mathf.Sin(_timer * 2.0f) * Mathf.Deg2Rad(10f), delta * 2.0f);
        }

        protected override void Collected(Player player)
        {
            if (_collected) return;
            _collected = true;
            player.Sfx.PlayEssenceSound(1f);
            _game.CurrentGameMode.EssenceCollected();
            PlayCollectedAnimation();
        }

        private void PlayCollectedAnimation()
        {
            _tween = CreateTween();
            _tween.SetTrans(Tween.TransitionType.Back);
            _tween.SetEase(Tween.EaseType.Out);
            _tween.TweenProperty(this, "scale", Vector2.One * 1.4f, 0.15f);
            _tween.TweenProperty(this, "scale", Vector2.One * 1f, 0.25f);
            _tween.Parallel();
            _tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0.0f), 0.25f);
            _tween.TweenCallback(this, nameof(Collect));

            GetNode<Particles2D>("%EssenceCollectedParticles").Restart();
        }

        private void Collect()
        {
            Visible = false;
        }

        protected override bool IsCollected() => _collected;

        private bool _collected;
        private float _timer;

        private Game _game;

        private SceneTreeTween _tween;

        [Export] private Color _challengeModeColor = new Color();
    }
}
