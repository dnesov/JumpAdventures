using Godot;

namespace Jump.Entities
{
    public class TrapSpike : ObstacleBase
    {
        public override void _Ready()
        {
            base._Ready();
            _currentState = _initialState;
            _offset *= GlobalScale.y;
            _tween = new Tween();
            AddChild(_tween);

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
            player.HealthHandler.Damage(1);
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
                    _player.HealthHandler.Damage(1);
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

        private void Appear()
        {
            _tween.RemoveAll();
            var target = new Vector2(GlobalPosition.x, GlobalPosition.y) - Vector2.Down.Rotated(Rotation) * _offset;
            _tween.InterpolateProperty(this, "position", GlobalPosition, target, _tweenDuration, Tween.TransitionType.Back, Tween.EaseType.Out);
            _tween.Start();
        }

        private void Disappear()
        {
            _tween.RemoveAll();
            var target = new Vector2(GlobalPosition.x, GlobalPosition.y) + Vector2.Down.Rotated(Rotation) * _offset;
            _tween.InterpolateProperty(this, "position", GlobalPosition, target, _tweenDuration, Tween.TransitionType.Back, Tween.EaseType.Out);
            _tween.Start();
        }

        private TrapSpikeState _currentState = TrapSpikeState.Hidden;
        [Export] private TrapSpikeState _initialState;
        [Export] private float _tweenDuration = 0.15f;
        [Export] private float _stayTime = 0.5f;
        [Export] private float _hiddenTime = 1f;
        private float _initialYPosition;
        private float _offset = 14f;
        private Tween _tween;
        private float _hiddenTimer, _stayTimer;
        private bool _playerHit;
        private Player _player;
    }
    public enum TrapSpikeState
    {
        Hidden,
        Visible
    }
}