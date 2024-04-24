using System;
using Godot;
using Jump.Extensions;

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

            ResetPosition();
            ReturnToPlayer();

            _game = this.GetSingleton<Game>();
        }
        public override void _ExitTree() => UnsubscribeEvents();

        public void AddShake(float amount)
        {
            var mul = _game.Data.Settings.AccessibilitySettings.ScreenShake;
            _trauma = Math.Min(_trauma + amount * mul, 1);
        }

        public override void _Process(float delta)
        {
            if (_trauma > 0)
            {
                _trauma = Math.Max(_trauma - Decay * delta, 0);
                Shake();
            }

            UpdateZoom(delta);
            // UpdateRotation(delta);
            FollowTarget(delta);
        }

        public void InterpolateTo(Camera2D targetCamera, float speedScale)
        {
            _smoothingSpeedScale = speedScale;
            _target = targetCamera;
        }

        public void ReturnToPlayer()
        {
            _target = _player;
            _smoothingSpeedScale = 1.0f;
        }

        public void ResetPosition() => GlobalPosition = _player.GlobalPosition;

        private void FollowTarget(float delta)
        {
            var targetPos = _target.GlobalPosition;

            var targetXInterpolated = Mathf.Lerp(GlobalPosition.x, targetPos.x, delta * _horizontalSpeed);
            var targetYInterpolated = Mathf.Lerp(GlobalPosition.y, targetPos.y, delta * _verticalSpeed);
            var targetInterpolated = new Vector2(targetXInterpolated, targetYInterpolated);

            GlobalPosition = targetInterpolated;
        }

        private void UpdateZoom(float delta)
        {
            Zoom = Zoom.LinearInterpolate(Vector2.One * _targetZoom, delta * _zoomSpeed);
        }


        private void Shake()
        {
            float amount = (float)Math.Pow(_trauma, _traumaPower);
            Offset = new Vector2(MaxOffset.x * amount * new Random().Next(-1, 1), MaxOffset.y * amount * new Random().Next(-1, 1));
        }

        private void UpdateRotation(float delta)
        {
            var target = Player.GravityFlipped ? Mathf.Deg2Rad(180f) : Mathf.Deg2Rad(0);

            GlobalRotation = Mathf.LerpAngle(GlobalRotation, target, delta * _rotationSpeed);
        }

        private void SubscribeToEvents()
        {
            Player.HealthHandler.OnDamage += OnDamage;
            Player.HealthHandler.OnDeath += OnDeath;
            Player.OnAnyRespawn += OnRespawn;
            Player.OnWin += OnWin;
        }
        private void UnsubscribeEvents()
        {
            Player.HealthHandler.OnDamage -= OnDamage;
            Player.OnAnyRespawn -= OnRespawn;
            Player.OnWin -= OnWin;
        }

        private void OnDamage(int damage, DamageType damageType) => AddShake(0.35f);
        private void OnDeath() => TargetZoom = _deathZoom;

        private void OnRespawn()
        {
            _zoomSpeed = _defaultZoomSpeed;
            TargetZoom = _defaultZoom;
        }

        private void OnWin()
        {
            _zoomSpeed = _winZoomSpeed;
            TargetZoom = _winZoom;
        }

        [Export] private Vector2 _scrollWindow = new Vector2(64f, 128f);

        private float _defaultZoom = 1.1f;
        private float _deathZoom = 0.9f;
        private float _trauma;
        private float _traumaPower = 2f;
        private Node2D _target;
        private float _targetZoom;
        private float _winZoom = 0.85f;

        private float _zoomSpeed = 3f;
        private float _defaultZoomSpeed = 3f;
        private float _winZoomSpeed = 6f;
        private float _horizontalSpeed = 10f;
        private float _verticalSpeed = 2f;
        private float _rotationSpeed = 2f;
        private float _smoothingSpeedScale = 1.0f;
        private Player _player;
        private Game _game;
    }
}