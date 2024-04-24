using Godot;
using GodotFmod;
using Jump.Extensions;

namespace Jump.Entities
{
    public class WindObstacle : ObstacleBase
    {
        public override void _Ready()
        {
            base._Ready();
            _fmodRuntime = this.GetSingleton<FmodRuntime>();
            _windLoopInstance = _fmodRuntime.GetEventInstance(_loopEventPath);
        }
        protected override void OnPlayerEntered(Player player)
        {
            if (_player == null) _player = player;

            _player.Effects.PlayWindEffect();
            PlayEnterSound();
            _player.Effects.IsInsideWind = true;
        }

        protected override void OnPlayerExited(Player player)
        {
            _player.Effects.IsInsideWind = false;
            PlayExitSound();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!playerInside) return;
            var dir = Vector2.Right.Rotated(GlobalRotation) * -1f;
            _player.Velocity += dir * _windForce;
        }

        private void PlayEnterSound()
        {
            if (_player.Effects.IsInsideWind) return;
            _fmodRuntime.PlayOneShot(_enterEventPath);
            _windLoopInstance.Start();
        }
        private void PlayExitSound()
        {
            if (_player.Effects.IsInsideWind) return;
            _fmodRuntime.PlayOneShot(_exitEventPath);
            _windLoopInstance.Stop();
        }

        [Export] private float _windForce = 40f;
        private Player _player;
        private FmodRuntime _fmodRuntime;

        private readonly string _enterEventPath = "event:/WindEnter";
        private readonly string _exitEventPath = "event:/WindExit";
        private readonly string _loopEventPath = "event:/WindLoop";

        private FmodEventInstance _windLoopInstance;
    }
}