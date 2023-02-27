using System;
using Godot;

namespace Jump.Entities
{
    public class PlayerCamera : Camera2D
    {
        public float Decay = .8f;
        public Vector2 MaxOffset = new Vector2(100, 75);
        public float TargetZoom { get => _targetZoom; set => _targetZoom = value; }
        public Player Player { get => _player; set => _player = value; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            SubscribeToEvents();
            _targetZoom = _defaultZoom;

            SetAsToplevel(true);

            GlobalPosition = _player.GlobalPosition;
        }
        public override void _ExitTree() => UnsubscribeEvents();

        public void AddShake(float amount) => _trauma = Math.Min(_trauma + amount, 1);

        public override void _PhysicsProcess(float delta)
        {
            if (_trauma > 0)
            {
                _trauma = Math.Max(_trauma - Decay * delta, 0);
                Shake();
            }

            UpdateZoom(delta);
            FollowPlayer(delta);
        }

        public void InterpolateTo(Camera2D target, float speed)
        {
            SetAsToplevel(true);
            SmoothingSpeed = speed;
            GlobalPosition = target.GlobalPosition;
        }

        public void ReturnToPlayer()
        {
            SetAsToplevel(false);
            GlobalPosition = _player.GlobalPosition;
            SmoothingSpeed = 10f;
        }

        public void ResetPosition()
        {
            GlobalPosition = _player.GlobalPosition;
        }

        private void FollowPlayer(float delta)
        {
            var target = _player.GlobalPosition;

            var targetXInterpolated = Mathf.Lerp(GlobalPosition.x, target.x, delta * _horizontalSpeed);
            var targetYInterpolated = Mathf.Lerp(GlobalPosition.y, target.y, delta * _verticalSpeed);
            var targetInterpolated = new Vector2(targetXInterpolated, targetYInterpolated);

            GlobalPosition = targetInterpolated;
        }

        private void UpdateZoom(float delta)
        {
            Zoom = Zoom.LinearInterpolate(Vector2.One * _targetZoom, delta * 3);
        }


        private void Shake()
        {
            float amount = (float)(Math.Pow(_trauma, _traumaPower));
            Offset = new Vector2(MaxOffset.x * amount * new Random().Next(-1, 1), MaxOffset.y * amount * new Random().Next(-1, 1));
        }

        private void SubscribeToEvents()
        {
            Player.HealthHandler.OnDamage += OnDamage;
            Player.HealthHandler.OnDeath += OnDeath;
            Player.OnAnyRespawn += OnRespawn;
        }
        private void UnsubscribeEvents()
        {
            Player.HealthHandler.OnDamage -= OnDamage;
            Player.OnAnyRespawn -= OnRespawn;
        }

        private void OnDamage(int damage) => AddShake(0.35f);
        private void OnDeath() => TargetZoom = _deathZoom;
        private void OnRespawn() => TargetZoom = _defaultZoom;


        [Export] private Vector2 _scrollWindow = new Vector2(64f, 128f);

        private float _defaultZoom = 1.1f;
        private float _deathZoom = 0.9f;
        private float _trauma;
        private float _traumaPower = 2f;
        private float _targetZoom;
        private float _horizontalSpeed = 10f;
        private float _verticalSpeed = 2f;
        private Player _player;
    }
}