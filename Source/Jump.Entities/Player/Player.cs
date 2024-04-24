using System;
using Godot;
using Jump.Extensions;
using Jump.Misc;
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
        public Vector2 SnapVector { get; set; }
        public readonly Vector2 GroundSnapVector = Vector2.Down * 16f;
        public Vector2 UpDirection { get => _upDirection; set => _upDirection = value; }
        public Vector2 VelocityBeforeLanding => _velocityBeforeLanding;

        public Vector2 SpawnPosition => _spawnPosition;

        public LevelRoot LevelRoot => GetOwner<LevelRoot>();

        public float LandThreshold => 130f;

        public bool JustLanded => _stateContext.PreviousStateEqualTo(typeof(AirState)) && Mathf.Abs(VelocityBeforeLanding.y) > LandThreshold;

        public Effects Effects => _effects;

        public bool GravityFlipped => _gravityFlipped;
        public bool JumpBuffered => _jumpBufferTimer > 0f;

        public Action OnAnyJump;
        public Action<bool> OnJump;
        public Action OnRetryRequested;
        public Action<bool> OnRespawn;
        public Action OnAnyRespawn;
        public Action OnWin;
        public Action OnLanded;

        public void BeginAdhesion()
        {
            _adheased = true;
            _stateContext.SwitchTo(new AdheasedState());
        }
        public void EndAdhesion()
        {
            _adheased = false;
            _stateContext.SwitchTo(new AirState());
        }
        public void BeginStick() => _stateContext.SwitchTo(new StickedState());
        public void EndStick() => _stateContext.SwitchTo(new AirState());

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _game = this.GetSingleton<Game>();
            _game.DisableCursor();

            ConstructSystems();
            AddChildren();

            _sprite = GetNode<PlayerSprite>(_playerSpritePath);
            _progressHandler = this.GetSingleton<ProgressHandler>();

            _soundtrackManager = this.GetSingleton<SoundtrackManager>();
            _controllerRumbler = this.GetSingleton<ControllerRumbler>();

            _gui = GetNode<PlayerGUI>("%PlayerGUI");

            SubscribeToEvents();

            ResetFriction();

            _spawnPosition = GlobalPosition;

            _effects = GetNode<Effects>("EffectsLayer/Effects");

            _soundtrackManager.CurrentTrackState = TrackState.InGame;

            _soundtrackManager.Intensity = 0.0f;
            _soundtrackManager.Damaged = 0.0f;
            _soundtrackManager.End = 0.0f;

            DisableInput();
            ShowWorldIntro();

            SnapVector = GroundSnapVector;

            _game.ResetTimer();
            _game.CurrentGameMode.RegisterPlayer(this);

#if DEBUG
            // yuck
            var debugOverlayParent = this.GetSingleton<CanvasLayer>("DebugOverlay");
            var debugOverlay = debugOverlayParent.GetNode<DebugOverlay>("ImGuiNode");
            debugOverlay.RegisterPlayer(this);
#endif
        }
        public override void _ExitTree() => Cleanup();

        public override void _PhysicsProcess(float delta)
        {
#if DEBUG
            // DrawDebug();
#endif

            if (_soundtrackManager.CurrentTrackState != TrackState.InGame) return;

            if (_velocity.Length() < _movementSettings.Speed / 4f)
            {
                _soundtrackManager.Intensity -= delta / 10f;
            }
            else
            {
                _soundtrackManager.Intensity -= delta / 20f;
            }
        }

        public override void _Process(float delta)
        {
            GetInput();
            ProcessMovement(delta);
            KillIfOutOfBounds();

            if (_game.TimerActive == false && HasStarted())
            {
                _game.TimerActive = true;
                _game.StartTimer();
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed("restart")) RequestRetry();
        }

        public void StartMoveLeft()
        {
            _input.Direction = Vector2.Left;
        }

        public void StopMoveLeft()
        {
            _input.Direction = Vector2.Zero;
        }

        public void StartMoveRight()
        {
            _input.Direction = Vector2.Right;
        }

        public void StopMoveRight()
        {
            _input.Direction = Vector2.Zero;
        }

        public void InputJump()
        {
            _input.Jumping = true;
        }

        public void Win()
        {
            DisableMovement();
            _soundtrackManager.Intensity = 0.5f;
            _soundtrackManager.End = 1.0f;

            var color = _sprite.Color;
            color.a = 0.25f;
            _effects.Flash(color, 2f);

            OnWin?.Invoke();
        }

        public void Jump(float speedMultiplier = 1.0f, bool shouldPlaySound = true)
        {
            ResetFriction();

            var jumpSpeed = GravityFlipped ? _movementSettings.JumpSpeed : -_movementSettings.JumpSpeed;
            _velocity.y = jumpSpeed * speedMultiplier;

            SnapVector = Vector2.Zero;

            OnAnyJump?.Invoke();
            OnJump?.Invoke(shouldPlaySound);

            _soundtrackManager.Intensity += 0.25f;

            _progressHandler.Jumps += 1;
            _game.AchievementProvider.AddStat(StatIds.JUMPS, 1);
        }

        public void FlipGravity()
        {
            _gravityFlipped = !_gravityFlipped;
            _upDirection = -_upDirection;
        }

        public void DisableMovement()
        {
            _canMove = false;
            _velocity = Vector2.Zero;
        }
        public void EnableMovement() => _canMove = true;

        public void EnableInput()
        {
            _inputEnabled = true;
        }

        public void DisableInput()
        {
            _inputEnabled = false;
            _input = new InputData();
        }

        public void Dash(Vector2 dir, float speed, float friction)
        {
            // _stateContext.SwitchTo(new AirState());
            _effects.Flash(new Color(0f, 1f, 0f, 0.1f));
            _stateContext.SwitchTo(new DashState(dir, speed));
        }

        public void ResetFriction() => Friction = _movementSettings.GroundFriction;
        public void ResetPosition() => GlobalPosition = _spawnPosition;

        public void RequestRetry() => OnRetryRequested?.Invoke();

        public void Respawn()
        {
            if (_game.CurrentState == GameState.PlayingOverWin) return;
            if (_healthHandler.Dead) Effects.ReverseDeathEffect();

            _velocity = Vector2.Zero;

            ResetPosition();
            EnableMovement();


            _camera.ResetPosition();
            _sprite.ResetPosition();

            _soundtrackManager.Damaged = 0.0f;
            _soundtrackManager.End = 0.0f;

            OnRespawn?.Invoke(true);
            OnAnyRespawn?.Invoke();


            if (_gravityFlipped) FlipGravity();
            _game.StopTimer();

        }

        public bool HasStarted()
        {
            return GlobalPosition.DistanceTo(_spawnPosition) > GAME_START_DISTANCE;
        }

        public bool ShouldIncrementAttempts()
        {
            return HasStarted() || _healthHandler.Dead;
        }

        private void ConstructSystems()
        {
            _healthHandler = new HealthHandler(_game.CurrentGameMode.MaxHearts);
            _inputHandler = new StandardInputHandler();

            _camera = new PlayerCamera()
            {
                Current = true,
                Player = this,
                Rotating = true
            };

            _animator = new PlayerAnimator();
            _sfx = new PlayerSfx(this);

            _stateContext = new StateContext(this, _movementSettings);
            _stateContext.SwitchTo(new IdleState());
        }

        private void SubscribeToEvents()
        {

            _healthHandler.OnAnyDamage += OnAnyDamage;
            _healthHandler.OnDeath += OnDeath;
            OnAnyRespawn += _healthHandler.Revive;

            _game.OnWin += Win;
        }

        private void Cleanup()
        {
            UnsubscribeEvents();
            QueueFree();
        }
        private void UnsubscribeEvents()
        {
            _game.OnWin -= Win;

            _healthHandler.OnAnyDamage -= OnAnyDamage;
            _healthHandler.OnDeath -= OnDeath;
            OnAnyRespawn -= _healthHandler.Revive;
        }

        private void OnAnyDamage(DamageType damageType)
        {
            _controllerRumbler.RumbleWeak(0.5f, 0.3f);
            _soundtrackManager.Damaged = 1.0f;
            _effects.PlayDamageEffect();
        }

        // TODO: move to a separate class.
        private void ShowWorldIntro()
        {
            // TODO: ouch.
            var worldIntroUI = GetNode<WorldIntroUI>("%WorldIntroUI");

            var worldIntroUIData = new WorldIntroData()
            {
                WorldName = _game.CurrentWorld.Name,
                LevelName = _game.CurrentLevel.Name
            };

            if (!_game.ShouldShowIntro())
            {
                Effects.HideIntroEffect();
                EnableInput();
                EnableMovement();
                return;
            }

            DisableInput();

            if (_game.JustStartedPlaying())
            {
                Effects.PlayIntroEffect();
                worldIntroUI.UpdateElements(worldIntroUIData);
            }
            else
            {
                Effects.PlayIntroEffect(alreadyPlaying: true);
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

        private void GetInput()
        {
            if (!_inputEnabled) return;
            _input = _inputHandler.GetInput();
        }

        private void ProcessMovement(float delta)
        {
            if (_input.Jumping) _jumpBufferTimer = _jumpBufferTime;
            var stateData = new PlayerStateData
            {
                Input = _input,
                Velocity = _velocity,
                OnGround = IsOnFloor(),
                Adheased = _adheased,
                GravityFlipped = _gravityFlipped,
                WasJumpPressed = _jumpBufferTimer > 0.0f,
            };

            _jumpBufferTimer -= delta;

            if (!_canMove) return;
            _stateContext.Process(stateData, out _velocity, delta);

            var floorNormal = GetFloorNormal();

            _velocity = MoveAndSlideWithSnap(_velocity, SnapVector, _upDirection, true, 4, Mathf.Deg2Rad(45), false);
            _velocity.Rotated(floorNormal.Angle() + Mathf.Pi / 2);
            if (!IsOnFloor()) _velocityBeforeLanding = _velocity;
            else _jumpBufferTimer = 0.0f;


            _justLanded = false;
        }

        private void DrawDebug()
        {
            DebugDraw.SetText(nameof(IsOnWall), IsOnWall());
            DebugDraw.SetText(nameof(IsOnFloor), IsOnFloor());
            DebugDraw.SetText("FloorVelocity", GetFloorVelocity());
            DebugDraw.SetText(nameof(_velocity), _velocity);
            DebugDraw.SetText(nameof(_sprite.Rotation), _sprite.Rotation);
            DebugDraw.SetText(nameof(_stateContext.CurrentState), _stateContext.CurrentState);
            DebugDraw.SetText(nameof(_adheased), _adheased);
        }

        private bool IsOutOfBounds() => GlobalPosition.y >= OUT_OF_BOUNDS_THRESHOLD;

        private void OnDeath()
        {
            _game.GameOver();
            _controllerRumbler.RumbleWeak(1.0f, 0.5f);

            // TODO: remove dependency on Effects.
            Effects.PlayDeathEffect();
            DisableMovement();

            _game.StopTimer();
        }

        private void KillIfOutOfBounds()
        {
            if (IsOutOfBounds()) _healthHandler.Kill();
        }



        #region Private Fields
        private StateContext _stateContext;
        private HealthHandler _healthHandler;
        private InputHandler _inputHandler;
        private ProgressHandler _progressHandler;
        private PlayerSprite _sprite;
        private PlayerVfx _vfx;
        private PlayerSfx _sfx;
        private PlayerCamera _camera;
        private PlayerGUI _gui;
        private PlayerAnimator _animator;
        private Effects _effects;
        private ControllerRumbler _controllerRumbler;
        private Game _game;

        private SoundtrackManager _soundtrackManager;
        [Export] private MovementSettings _movementSettings;
        private InputData _input;
        private Vector2 _velocity;
        private Vector2 _velocityBeforeLanding;
        private float _friction;
        private bool _adheased;
        private bool _justLanded;
        private bool _inputEnabled = true;
        private bool _gravityFlipped;
        private Vector2 _upDirection = Vector2.Up;
        [Export] private NodePath _playerSpritePath;
        [Export] private PackedScene _deathParticlesScene, _winParticlesScene;
        [Export] private PackedScene _hurtParticlesScene;

        private float _jumpBufferTime = 1f;
        private float _jumpBufferTimer;

        private Color _color;
        private bool _canMove = true;
        private Vector2 _spawnPosition;
        private const float GAME_START_DISTANCE = 128f;
        private const float OUT_OF_BOUNDS_THRESHOLD = 8000f;
        #endregion
    }
}