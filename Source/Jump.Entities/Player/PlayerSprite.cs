using Godot;
using Jump.Customize;

namespace Jump.Entities
{
    // [Tool]
    /// <summary>
    /// Player visual representation.
    /// </summary>
    ///
    public class PlayerSprite : Sprite
    {
        [Export]
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                Modulate = value;
            }
        }

        // Called when the node enters the scene tree for the first time.
        public override async void _Ready()
        {
            _player = GetNode<Player>(_playerPath);

            _initialScale = Scale;

            _walkParticles = GetNode<Particles2D>(_walkParticlesPath);
            UpdateCrackShader(0.0f);

            await ToSignal(_player, "ready");
            _customizationHandler = GetTree().Root.GetNode<CustomizationHandler>("CustomizationHandler");
            SubscribeEvents();

            var preferences = _customizationHandler.Preferences;
            var color = _customizationHandler.GetColorById(preferences.ColorId);
            var skin = _customizationHandler.GetSkinById(preferences.SkinId);

            Color = color;
            Texture = skin;
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        public override void _PhysicsProcess(float delta)
        {
            Animate(delta);
        }

        public void ResetPosition()
        {
            Position = _player.Position;
        }

        private void Animate(float delta)
        {
            Position = Position.LinearInterpolate(new Vector2(_player.Position.x, _player.Position.y - Offset.y), delta * 45f);
            Rotation = _targetRotation;

            if (_player.IsOnFloor())
                OnFloor(delta);

            if (!_player.IsOnFloor())
                NotOnFloor(delta);

            _targetRotation = Mathf.LerpAngle(_targetRotation + _jumpRotation, _surfaceAngle, delta * 10);
            _jumpRotation = Mathf.LerpAngle(_jumpRotation, 0, delta * 10);

            UpdateParticles(Mathf.Abs(_player.Velocity.x));
            _prevVelocity = _player.Velocity;

            _targetScale.x = Mathf.Lerp(_targetScale.x, 1f, 1f - Mathf.Pow(_squishAndStretchExponent, delta));
            _targetScale.y = Mathf.Lerp(_targetScale.y, 1f, 1f - Mathf.Pow(_squishAndStretchExponent, delta));

            Scale = _targetScale * _initialScale;
        }

        private void OnFloor(float delta)
        {
            var surfaceNormal = _player.GetFloorNormal();
            var angle = GetSurfaceAngle(surfaceNormal);
            UpdateSurfaceAngle(angle);

            if (!_landed && _player.IsOnFloor())
            {
                _landed = true;

                _targetScale.x = Mathf.RangeLerp(Mathf.Abs(_prevVelocity.y), 0, _player.MaxSpeed, _landSquashStrengthMin, _landSquashStrengthMax);
                _targetScale.y = Mathf.RangeLerp(Mathf.Abs(_prevVelocity.y), 0, _player.MaxSpeed, _landSquashStrengthMax, _landSquashStrengthMin);
            }
        }

        private void NotOnFloor(float delta)
        {
            _landed = false;
            _surfaceAngle = Mathf.LerpAngle(_surfaceAngle, 0, delta * 5);

            Vector2 scale;
            scale.y = Mathf.RangeLerp(Mathf.Abs(_player.Velocity.y), 0, Mathf.Abs(_player.JumpSpeed), _airStretchStrengthMin, _airStretchStrengthMax);
            scale.x = Mathf.RangeLerp(Mathf.Abs(_player.Velocity.y), 0, Mathf.Abs(_player.JumpSpeed), _airStretchStrengthMax, _airStretchStrengthMin);

            _targetScale = scale;
        }

        private float GetSurfaceAngle(Vector2 surfaceNormal)
        {
            return surfaceNormal.Angle() + Mathf.Pi / 2f;
        }

        public void UpdateSurfaceAngle(float angle) => _surfaceAngle = angle;

        public void UpdateCrackShader(float amount)
        {
            var material = (ShaderMaterial)Material;
            material.SetShaderParam("damage_amount", amount);
        }

        public void SubscribeEvents()
        {
            _player.OnJump += (direction) => { Jump(direction); };

            _player.HealthHandler.OnAnyDamage += OnDamage;
            _player.HealthHandler.OnDeath += OnDeath;
            _player.OnAnyRespawn += OnRespawn;
            _player.OnWin += OnWin;

            _customizationHandler.OnPreferencesChanged += ApplyCustomization;
        }

        private void UnsubscribeEvents()
        {
            _customizationHandler.OnPreferencesChanged -= ApplyCustomization;
        }

        private void OnDamage()
        {
            var healthHandler = _player.HealthHandler;
            var damageAmount = Mathf.Abs(((float)healthHandler.Health / (float)healthHandler.MaxHealth - 1.0f) * 2.0f);

            damageAmount = Mathf.Clamp(damageAmount, 0, 2);
            UpdateCrackShader(damageAmount);
        }

        private void OnDeath() => Visible = false;

        private void OnRespawn()
        {
            Visible = true;
            UpdateCrackShader(0.0f);
        }

        private void OnWin() => Visible = false;

        private void Jump(Vector2 direction)
        {
            _jumpDirection = direction;
            _jumpRotation += direction.x * Mathf.Deg2Rad(5);
        }

        private void UpdateParticles(float playerSpeed)
        {
            _walkParticles.Emitting = playerSpeed > _player.MaxSpeed / 2 && _player.IsOnFloor();
        }

        private float EaseOutBack(float t)
        {
            var c1 = 1.70158f;
            var c3 = c1 + 1;

            return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
        }

        public void ApplyCustomization(CustomizationPreferences prefs)
        {
            var skinId = prefs.SkinId;
            var colorId = prefs.ColorId;

            var color = _customizationHandler.GetColorById(colorId);
            var skin = _customizationHandler.GetSkinById(skinId);

            Color = color;
            Texture = skin;
        }

        private Vector2 _initialScale;
        private bool _landed;
        private Vector2 _prevVelocity;
        private Vector2 _jumpDirection;
        private Color _color;
        [Export] private NodePath _playerPath;
        [Export] private NodePath _walkParticlesPath;
        private float _targetRotation;

        private Vector2 _targetScale;
        private float _jumpRotation;
        private float _surfaceAngle;

        [Export] private float _landSquashStrengthMin = 0.9f;
        [Export] private float _landSquashStrengthMax = 1.1f;
        [Export] private float _airStretchStrengthMin = 0.8f;
        [Export] private float _airStretchStrengthMax = 1.2f;
        [Export] private float _squishAndStretchExponent = 0.05f;

        #region Nodes
        private Player _player;
        private Particles2D _walkParticles;
        private CustomizationHandler _customizationHandler;
        #endregion
    }
}
