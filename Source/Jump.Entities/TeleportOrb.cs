using Godot;
using GodotFmod;

namespace Jump.Entities
{
    public class TeleportOrb : Area2D, IObstacle
    {
        public override void _Ready()
        {
            _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
            _line = GetNode<Line2D>(_linePath);

            _line.AddPoint(GlobalPosition);

            if (IsTarget)
            {
                Modulate = _bColor;
                return;
            }
            _target = GetNode<TeleportOrb>(_teleportTarget);
            _target.IsTarget = true;

            _line.AddPoint(_target.GlobalPosition);
        }
        public void PlayerEntered(Player player)
        {
            _player = player;
            _playerInside = true;

            _fmodRuntime.PlayOneShot(_touchSoundPath);
        }

        public void PlayerExited(Player player)
        {
            _playerInside = false;
            _activated = false;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_playerInside && !_activated && Input.IsActionJustPressed("move_jump"))
            {
                if (IsTarget) return;
                _activated = true;
                _player.GlobalPosition = _target.GlobalPosition;

                _fmodRuntime.PlayOneShot(_bounceSoundPath);
            }
        }

        private FmodRuntime _fmodRuntime;
        [Export] private string _touchSoundPath = "event:/OrbTouch";
        [Export] private string _bounceSoundPath = "event:/Bounce";

        private bool _playerInside;
        private bool _activated;
        private Player _player;

        private readonly Color _bColor = new Color("4c9ef9");
        [Export] private NodePath _linePath;
        private Line2D _line;

        [Export] private NodePath _teleportTarget;
        private TeleportOrb _target;

        public bool IsTarget { get; set; }
    }
}
