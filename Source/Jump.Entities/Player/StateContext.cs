using System;
using Godot;

namespace Jump.Entities
{
    public class StateContext
    {
        public PlayerState CurrentState => _currentState;
        public PlayerState PreviousState => _previousState;
        public PlayerStateData StateData { get => _stateData; set => _stateData = value; }
        public Player Player { get => _player; set => _player = value; }
        public MovementSettings MovementSettings => _movementSettings;

        public StateContext(Player player, MovementSettings movementSettings)
        {
            Player = player;
            _movementSettings = movementSettings;
        }

        public void SwitchTo(PlayerState state)
        {
            if (state == _currentState) return;
            _previousState = _currentState;
            _previousState?.OnExit(this);
            _currentState = state;
            CurrentState.OnEnter(this);
        }

        public bool StateEqualTo(PlayerState a, Type b) => a.GetType() == b;
        public bool PreviousStateEqualTo(Type b) => PreviousState.GetType() == b;

        public void Process(PlayerStateData stateData, out Vector2 velocity, float delta)
        {
            _stateData = stateData;
            _currentState.OnProcess(this, out velocity, delta);
        }

        private Player _player;
        private MovementSettings _movementSettings;
        private PlayerState _currentState;
        private PlayerState _previousState;
        private PlayerStateData _stateData;
    }
}