using System;
using Godot;
using Jump.UI;
using Jump.Utils;

namespace Jump.Entities
{
    /// <summary>
    /// Player controllable entity.
    /// </summary>
    public class Player : KinematicBody2D
    {
        public HealthHandler HealthHandler => _healthHandler;
        public PlayerCamera Camera => _camera;
        public PlayerSfx Sfx => _sfx;
        public PlayerSprite Sprite => _sprite;
        public PlayerGUI GUI => _gui;
        public float MaxSpeed => _movementSettings.Speed;
        public float JumpSpeed => _movementSettings.JumpSpeed;
        public Vector2 Velocity { get => _velocity; set => _velocity = value; }
        public Vector2 MoveDirection => Velocity.Normalized();
        public Vector2 InputDirection => _input.Direction;
        public float Friction { get => _friction; set => _friction = value; }
        public Vector2 SnapVector => !_stateContext.StateData.Input.Jumping ? Vector2.Down * 16f : Vector2.Zero;
        public Vector2 UpDirection { get => _upDirection; set => _upDirection = value; }
        public Vector2 VelocityBeforeLanding => _velocityBeforeLanding;

        public float LandThreshold => 130f;

        public bool JustLanded => _stateContext.PreviousStateEqualTo(typeof(AirState)) && Mathf.Abs(VelocityBeforeLanding.y) > LandThreshold;

        public Action<Vector2> OnJump;
        public Action OnAnyJump;
        public Action OnRetryRequested;
        public Action<bool> OnRespawn;
        public Action OnAnyRespawn;
        public Action OnWin;

        public void BeginAdhesion() => _stateContext.SwitchTo(new AdheasedState());
        public void EndAdhesion() => _stateContext.SwitchTo(new AirState());
        public void BeginStick() => _stateContext.SwitchTo(new StickedState());
        public void EndStick() => _stateContext.SwitchTo(new AirState());

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            ConstructSystems();
            AddChildren();
            ShowWorldIntro();

            _sprite = GetNode<PlayerSprite>(_playerSpritePath);
            _healthContainer = GetNode<HealthContainer>(_healthContainerPath);

            // TODO: move this to a separate GUI class.
            _healthContainer.PopulateHealthSprites(_healthHandler.Health, _healthHandler.MaxHealth);

            _soundtrackManager = GetTree().Root.GetNode<SoundtrackManager>("SoundtrackManager");
            _gui = GetNode<PlayerGUI>("%PlayerGUI");

            SubscribeToEvents();

            ResetFriction();

            _spawnPosition = GlobalPosition;

            _effects = GetNode<Effects>("EffectsLayer/Effects");

            _soundtrackManager.CurrentTrackState = TrackState.InGame;
            _soundtrackManager.Intensity = 0.0f;
        }
        public override void _ExitTree() => UnsubscribeEvents();

        public override void _PhysicsProcess(float delta)
        {
#if DEBUG
            // DrawDebug();
#endif
        }

        public override void _Process(float delta)
        {
            GetInput();
            ProcessMovement(delta);
            KillIfOutOfBounds();
        }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed("restart")) RequestRetry();
        }

        public void Win()
        {
            DisableMovement();
            _soundtrackManager.Intensity = 0.5f;
            OnWin?.Invoke();
        }

        public void Jump(float speedMultiplier = 1.0f)
        {
            ResetFriction();
            _velocity.y = -_movementSettings.JumpSpeed * speedMultiplier;

            OnJump?.Invoke(MoveDirection);
            OnAnyJump?.Invoke();
            _soundtrackManager.Intensity += 0.5f;
        }

        public void DisableMovement() => _canMove = false;
        public void EnableMovement() => _canMove = true;
        public void Dash(Vector2 dir, float speed, float friction) => _stateContext.SwitchTo(new DashState(dir, speed));

        public void ResetFriction() => Friction = _movementSettings.GroundFriction;
        public void ResetPosition() => GlobalPosition = _spawnPosition;

        public void RequestRetry() => OnRetryRequested?.Invoke();

        public void RespawnAndRetry()
        {
            var game = GetTree().Root.GetNode<Game>("Game");
            if (game.CurrentState == GameState.PlayingOverWin) return;
            game.TryRetry(ShouldIncrementAttempts());
            if (_healthHandler.Dead) _effects.ReverseDeathEffect();

            EnableMovement();

            ResetPosition();
            _camera.ResetPosition();
            _sprite.ResetPosition();

            OnRespawn?.Invoke(true);
            OnAnyRespawn?.Invoke();

            _velocity = Vector2.Zero;
        }

        private void ConstructSystems()
        {
            _healthHandler = new HealthHandler();
            _inputHandler = new StandardInputHandler();

            _vfx = new PlayerVfx
            (
                _hurtParticlesScene,
                GetNode<Particles2D>(_walkParticlesPath),
                _deathParticlesScene,
                _winParticlesScene,
                this
            );

            _camera = new PlayerCamera()
            {
                Current = true,
                Player = this
            };

            _animator = new PlayerAnimator();
            _sfx = new PlayerSfx(this);

            _stateContext = new StateContext(this, _movementSettings);
            _stateContext.SwitchTo(new IdleState());
        }

        private void SubscribeToEvents()
        {
            var game = GetTree().Root.GetNode<Game>("Game");

            _healthHandler.OnDeath += OnDeath;
            OnAnyRespawn += _healthHandler.Revive;

            game.OnWin += Win;
            // game.OnRetry += Respawn;
        }

        private void UnsubscribeEvents()
        {
            var game = GetTree().Root.GetNode<Game>("Game");
            game.OnWin -= Win;
        }

        // TODO: move to a separate class.
        private void ShowWorldIntro()
        {
            // TODO: ouch.
            var game = GetTree().Root.GetNode<Game>("Game");
            var worldIntroUI = GetNode<WorldIntroUI>("%WorldIntroUI");

            var worldIntroUIData = new WorldIntroData()
            {
                WorldName = game.CurrentWorld.Name,
                LevelName = game.CurrentLevel.Name
            };

            if (!game.ShouldShowIntro()) return;
            if (game.JustStartedPlaying())
            {
                worldIntroUI.UpdateElements(worldIntroUIData);
            }
            else
            {
                worldIntroUIData.DisplayAnimTimeScale = 2.0f;
                worldIntroUI.UpdateElements(worldIntroUIData);
            }

            worldIntroUI.Display();
        }

        private void AddChildren()
        {
            AddChild(_camera);
            AddChild(_vfx);
            AddChild(_sfx);
            AddChild(_healthHandler);
        }

        private void GetInput() => _input = _inputHandler.GetInput();

        private void ProcessMovement(float delta)
        {
            if (_input.Jumping) _jumpBufferTimer = _jumpBufferTime;
            var stateData = new PlayerStateData
            {
                Input = _input,
                Velocity = _velocity,
                OnGround = IsOnFloor(),
                Adheased = _adheased,
                WasJumpPressed = _jumpBufferTimer > 0.0f,
            };

            _jumpBufferTimer -= delta;

            if (!_canMove) return;
            _stateContext.Process(stateData, out _velocity, delta);

            var floorNormal = GetFloorNormal();
            _velocity.Rotated(floorNormal.Angle() + Mathf.Pi / 2);

            _velocity = MoveAndSlideWithSnap(_velocity, SnapVector, _upDirection, true, 4, Mathf.Deg2Rad(45), false);
            if (!IsOnFloor()) _velocityBeforeLanding = _velocity;
            else _jumpBufferTimer = 0.0f;


            _justLanded = false;
        }

        private void DrawDebug()
        {
            DebugDraw.SetText(nameof(IsOnWall), IsOnWall());
            DebugDraw.SetText(nameof(_velocity), _velocity);
        }

        private bool IsOutOfBounds() => GlobalPosition.y >= OUT_OF_BOUNDS_THRESHOLD;

        private void OnDeath()
        {
            var game = GetTree().Root.GetNode<Game>("Game");
            game.GameOver();

            // TODO: remove dependency on Effects.
            _effects.PlayDeathEffect();
            DisableMovement();
        }

        private bool ShouldIncrementAttempts()
        {
            return GlobalPosition.DistanceTo(_spawnPosition) > ATTEMPT_INCREMENT_DIST
            || _healthHandler.Dead;
        }

        private void KillIfOutOfBounds()
        {
            if (IsOutOfBounds()) _healthHandler.Kill();
        }

        #region Private Fields
        private StateContext _stateContext;
        private HealthHandler _healthHandler;
        private InputHandler _inputHandler;
        private PlayerSprite _sprite;
        private PlayerVfx _vfx;
        private PlayerSfx _sfx;
        private PlayerCamera _camera;
        private PlayerGUI _gui;
        private HealthContainer _healthContainer;
        private PlayerAnimator _animator;
        private Effects _effects;

        private SoundtrackManager _soundtrackManager;
        [Export] private MovementSettings _movementSettings;
        private InputData _input;
        private Vector2 _velocity;
        private Vector2 _velocityBeforeLanding;
        private float _friction;
        private bool _adheased;
        private bool _justLanded;
        private Vector2 _upDirection = Vector2.Up;
        [Export] private NodePath _playerSpritePath;
        [Export] private NodePath _walkParticlesPath;
        [Export] private NodePath _healthContainerPath;
        [Export] private PackedScene _deathParticlesScene, _winParticlesScene;
        [Export] private PackedScene _hurtParticlesScene;

        private float _jumpBufferTime = 0.7f;
        private float _jumpBufferTimer;

        private Color _color;
        private bool _canMove = true;
        private Vector2 _spawnPosition;
        private const float ATTEMPT_INCREMENT_DIST = 128f;
        private const float OUT_OF_BOUNDS_THRESHOLD = 8000f;
        #endregion
    }
}