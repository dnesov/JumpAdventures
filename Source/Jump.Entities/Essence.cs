using Godot;
using System;

namespace Jump.Entities
{
    public class Essence : Collectible
    {
        public override void _Ready()
        {
            base._Ready();
            _game = GetTree().Root.GetNode<Game>("Game");
            SubscribeEvents();
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
            _game.OnRetry += Respawn;
        }

        private void UnsubscribeEvents()
        {
            _game.OnRetry -= Respawn;
        }

        private void Respawn()
        {
            _collected = false;
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
            player.Sfx.PlayEssenceSound();
            _game.SessionEssence++;
            PlayCollectedAnimation();
        }

        private void PlayCollectedAnimation()
        {
            var tween = CreateTween();
            tween.SetTrans(Tween.TransitionType.Back);
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "scale", Vector2.One * 1.4f, 0.15f);
            tween.TweenProperty(this, "scale", Vector2.One * 1f, 0.25f);
            tween.Parallel();
            tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0.0f), 0.25f);
            tween.TweenCallback(this, nameof(Collect));

            GetNode<Particles2D>("%CollectedParticles").Restart();
        }

        private void Collect()
        {
            Visible = false;
        }

        protected override bool IsCollected() => _collected;
        private bool _collected;
        private float _timer;

        private Game _game;
    }
}
