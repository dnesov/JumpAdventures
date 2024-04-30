using Godot;
using Jump.Customize;
using Jump.Extensions;

namespace Jump.Entities
{
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

        public PlayerTrail Trail { get => _currentTrail; set => _currentTrail = value; }

        public override async void _Ready()
        {
            _player = GetOwner<Player>();

            _walkParticles = GetNode<Particles2D>("Walkdust");
            _dashParticles = GetNode<Particles2D>("%DashParticles");
            _adheasedParticles = GetNode<Particles2D>("%AdheasedParticles");
            _adheasedEnteredParticles = GetNode<Particles2D>("%AdheasedEnteredParticles");

            _initialScale = Scale;
            UpdateCrackShader(0.0f);

            if (Engine.EditorHint) return;
            await ToSignal(_player, "ready");

            GlobalPosition = _player.GlobalPosition;
            _customizationHandler = this.GetSingleton<CustomizationHandler>();
            SubscribeEvents();

            var preferences = _customizationHandler.Preferences;
            ApplyCustomization(preferences);
            _currentTrail.ClearPoints();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        public override void _PhysicsProcess(float delta)
        {
            Update(delta);
            AnimateRotation(delta);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            AnimatePosition(delta);
            AnimateScale(delta);
        }

        public void ResetPosition()
        {
            Position = _player.Position;
        }

        public void ApplyCustomization(CustomizationPreferences prefs)
        {
            var skinId = prefs.SkinId;
            var colorId = prefs.ColorId;
            var trailId = prefs.TrailId;

            var color = _customizationHandler.GetColorById(colorId);
            var skin = _customizationHandler.GetSkinById(skinId);

            Color = color;
            Texture = skin;

            if (_currentTrail != null)
            {
                _currentTrail.QueueFree();
            }

            var trailScene = _customizationHandler.GetTrailSceneById(trailId);

            if (trailScene != null)
            {
                _currentTrail = trailScene.Instance<PlayerTrail>();
            }

            AddChild(_currentTrail);
        }

        public void EnableDashEffect()
        {
            _dashParticles.Emitting = true;
        }

        public void DisableDashEffect()
        {
            _dashParticles.Emitting = false;
        }

        public void EnableAdheasedEffect()
        {
            _adheasedEnteredParticles.Restart();

            if (!_adheasedParticles.Emitting)
            {
                _adheasedParticles.Restart();
            }
            else
            {
                _adheasedParticles.Emitting = true;
            }
        }

        public void DisableAdheasedEffect()
        {
            _adheasedParticles.Emitting = false;
        }

        public void UpdateCrackShader(float amount)
        {
            var material = (ShaderMaterial)Material;
            material.SetShaderParam("damage_amount", amount);
        }

        public void SubscribeEvents()
        {
            _player.OnAnyJump += Jump;
            _player.HealthHandler.OnAnyDamage += OnDamage;
            _player.HealthHandler.OnDeath += OnDeath;
            _player.OnAnyRespawn += OnRespawn;
            _player.OnWin += OnWin;

            _customizationHandler.OnPreferencesChanged += ApplyCustomization;
        }

        private void Update(float delta)
        {
            Rotation = _targetRotation;

            if (_player.IsOnFloor())
                OnFloor();

            if (!_player.IsOnFloor())
                NotOnFloor(delta);

            UpdateEffects(Mathf.Abs(_player.Velocity.x));
            _prevVelocity = _player.Velocity;
        }

        private void AnimatePosition(float delta)
        {
            var factor = DeltaLerpFactor(delta, _positionLerpFactor);
            GlobalPosition = GlobalPosition.LinearInterpolate(_player.GlobalPosition, factor);
        }

        private void AnimateScale(float delta)
        {
            _targetScale.x = Mathf.Lerp(_targetScale.x, 1f, 1f - Mathf.Pow(_squishAndStretchExponent, delta));
            _targetScale.y = Mathf.Lerp(_targetScale.y, 1f, 1f - Mathf.Pow(_squishAndStretchExponent, delta));

            Scale = _targetScale * _initialScale;
        }

        private void AnimateRotation(float delta)
        {
            var factor = DeltaLerpFactor(delta, _rotationLerpFactor);
            _targetRotation = Mathf.LerpAngle(_targetRotation + _jumpRotation, _surfaceAngle, factor);
            _jumpRotation = Mathf.LerpAngle(_jumpRotation, 0, factor);
        }

        private void OnFloor()
        {
            var surfaceNormal = _player.GetFloorNormal();
            var angle = GetSurfaceAngle(surfaceNormal);
            _surfaceAngle = angle;

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

            var initAngle = _player.GravityFlipped ? 180f : 0f;
            initAngle = Mathf.Deg2Rad(initAngle);
            _surfaceAngle = Mathf.LerpAngle(_surfaceAngle, initAngle, delta * 5);

            Vector2 scale;
            scale.y = Mathf.RangeLerp(Mathf.Abs(_player.Velocity.y), 0, Mathf.Abs(_player.JumpSpeed), _airStretchStrengthMin, _airStretchStrengthMax);
            scale.x = Mathf.RangeLerp(Mathf.Abs(_player.Velocity.y), 0, Mathf.Abs(_player.JumpSpeed), _airStretchStrengthMax, _airStretchStrengthMin);

            _targetScale = scale;
        }

        private float GetSurfaceAngle(Vector2 surfaceNormal)
        {
            return surfaceNormal.Angle() + Mathf.Pi / 2f;
        }

        private void UnsubscribeEvents()
        {
            _player.OnAnyJump -= Jump;
            _player.HealthHandler.OnAnyDamage -= OnDamage;
            _player.HealthHandler.OnDeath -= OnDeath;
            _player.OnAnyRespawn -= OnRespawn;
            _player.OnWin -= OnWin;

            _customizationHandler.OnPreferencesChanged -= ApplyCustomization;
        }

        private void OnDamage(DamageType damageType)
        {
            var healthHandler = _player.HealthHandler;
            var damageAmount = Mathf.Abs((float)healthHandler.Hearts / healthHandler.MaxHearts - 1.0f);

            damageAmount = Mathf.Clamp(damageAmount, 0, 1);
            UpdateCrackShader(damageAmount);
        }

        private void OnDeath()
        {
            Visible = false;
            _currentTrail.Emitting = false;
        }

        private void OnRespawn()
        {
            Visible = true;
            _currentTrail.Emitting = true;
            UpdateCrackShader(0.0f);
            _currentTrail.ClearPoints();
        }

        private void OnWin()
        {
            Visible = false;
            _currentTrail.Emitting = false;
        }

        private void Jump()
        {
            var dir = _player.InputDirection.Normalized();

            if (_player.GravityFlipped)
            {
                _jumpRotation -= dir.x * Mathf.Deg2Rad(_jumpRotationDegrees);
            }
            else
            {
                _jumpRotation += dir.x * Mathf.Deg2Rad(_jumpRotationDegrees);
            }
        }

        private void UpdateEffects(float playerSpeed)
        {
            var shouldEmit = playerSpeed > _player.MaxSpeed / 2;
            _walkParticles.Emitting = shouldEmit && _player.IsOnFloor();
            _currentTrail.Emitting = shouldEmit;
        }

        private float DeltaLerpFactor(float delta, float factor) => Mathf.Clamp(delta * factor, 0f, 1f);

        private float _positionLerpFactor = 45f;
        private float _rotationLerpFactor = 10f;
        private Vector2 _initialScale;
        private bool _landed;
        private Vector2 _prevVelocity;
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
        private float _jumpRotationDegrees = 10f;

        #region Nodes
        private Player _player;
        private Particles2D _walkParticles;
        private CustomizationHandler _customizationHandler;
        private PlayerTrail _currentTrail;
        private Particles2D _dashParticles, _adheasedParticles, _adheasedEnteredParticles;
        #endregion
    }
}
