using Godot;
using GodotFmod;

namespace Jump.Entities
{
    public class TrapSpike : ResetableObstacle
    {
        public override void _Ready()
        {
            base._Ready();
            _currentState = _initialState;
            _offset *= GlobalScale.y;
            _tween = new Tween();
            AddChild(_tween);

            _appearPlayer = GetNode<FmodEventPlayer2D>("AppearEvent");
            _disappearPlayer = GetNode<FmodEventPlayer2D>("DisappearEvent");

            _indicator = GetNode<TextureProgress>("%Indicator");
            _indicator.SetAsToplevel(true);

            _indicator.RectGlobalPosition = GetNode<Position2D>("%IndicatorPosition").GlobalPosition;

            _visiblePosition = GlobalPosition - Vector2.Down.Rotated(Rotation);
            _hiddenPosition = GlobalPosition + Vector2.Down.Rotated(Rotation) * _offset;

            _initialModulate = Modulate;
            _hiddenModulate = _initialModulate.Darkened(0.6f);

            _indicator.Modulate = _initialModulate;

            if (_currentState == TrapSpikeState.Hidden)
                Disappear();
            else if (_currentState == TrapSpikeState.Visible)
                Appear();
        }

        protected override void OnPlayerEntered(Player player)
        {
            if (_player == null)
                _player = player;

            if (_currentState == TrapSpikeState.Hidden) return;
            player.HealthHandler.Damage(1, 0.3f, DamageType.Spike);
        }

        protected override void OnPlayerExited(Player player) { }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_currentState == TrapSpikeState.Hidden)
                _hiddenTimer += delta;

            if (_currentState == TrapSpikeState.Visible)
            {
                _stayTimer += delta;
                if (!_playerHit && playerInside)
                {
                    _playerHit = true;
                    _player.HealthHandler.Damage(1, 0.3f, DamageType.Spike);
                }
            }

            if (_hiddenTimer >= _hiddenTime)
            {
                _currentState = TrapSpikeState.Visible;
                Appear();

                _stayTimer = 0f;
                _hiddenTimer = 0f;
            }

            if (_stayTimer >= _stayTime)
            {
                _currentState = TrapSpikeState.Hidden;
                Disappear();

                _stayTimer = 0f;
                _hiddenTimer = 0f;
                _playerHit = false;
            }
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            _indicator.Value = Mathf.Clamp(_hiddenTimer / _hiddenTime, 0f, 1f);
        }

        private void Appear()
        {
            _tween.RemoveAll();
            _tween.InterpolateProperty(this, "global_position", _hiddenPosition, _visiblePosition, _tweenDuration, Tween.TransitionType.Back, Tween.EaseType.Out);
            _tween.InterpolateProperty(this, "modulate", Modulate, _initialModulate, _tweenDuration, Tween.TransitionType.Back, Tween.EaseType.Out);
            _tween.Start();

            if (_playSounds)
            {
                _appearPlayer.Start();
            }
        }

        private void Disappear()
        {
            _tween.RemoveAll();
            var target = _hiddenPosition;
            _tween.InterpolateProperty(this, "global_position", _visiblePosition, _hiddenPosition, _tweenDuration, Tween.TransitionType.Back, Tween.EaseType.Out);
            _tween.InterpolateProperty(this, "modulate", Modulate, _hiddenModulate, _tweenDuration, Tween.TransitionType.Back, Tween.EaseType.Out);
            _tween.Start();

            if (_playSounds)
            {
                _disappearPlayer.Start();
            }
        }

        protected override void OnRestart()
        {
            _currentState = _initialState;
            _hiddenTimer = 0f;
            _stayTimer = 0f;

            if (_initialState == TrapSpikeState.Hidden)
            {
                Disappear();
            }
            else if (_initialState == TrapSpikeState.Visible)
            {
                Disappear();
            }
        }

        private void ScreenEntered()
        {
            _playSounds = true;
        }

        private void ScreenExited()
        {
            _playSounds = false;
        }

        private TrapSpikeState _currentState = TrapSpikeState.Hidden;
        [Export] private TrapSpikeState _initialState;
        [Export] private float _tweenDuration = 0.15f;
        [Export] private float _stayTime = 0.5f;
        [Export] private float _hiddenTime = 1f;
        private float _offset = 14f;
        private Tween _tween;
        private float _hiddenTimer, _stayTimer;
        private bool _playerHit;
        private Player _player;

        private Vector2 _hiddenPosition, _visiblePosition;
        private Color _initialModulate;
        private Color _hiddenModulate;

        private TextureProgress _indicator;

        private FmodEventPlayer2D _appearPlayer, _disappearPlayer;

        private bool _playSounds;
    }
    public enum TrapSpikeState
    {
        Hidden,
        Visible
    }
}