using Godot;
using System;

namespace Jump.Entities
{
    public class PlayerTrail : Line2D
    {
        public bool Emitting { get; set; }
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _parent = GetParent<PlayerSprite>();
            _particles = GetChild<Particles2D>(0);

            if (!_hasParticles)
            {
                _particles.Visible = false;
                _particles.Emitting = false;
            }

            SetAsToplevel(true);

            if (_usePlayerModulate)
            {
                Modulate = _parent.Modulate;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            AddPoint(_parent.GlobalPosition);

            while (GetPointCount() > _maxPointCount)
            {
                if (GetPointCount() == 0) return;
                RemovePoint(0);
            }

            if (_hasParticles)
            {
                _particles.Emitting = Emitting;
            }
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (_hasParticles)
            {
                _particles.GlobalPosition = _parent.GlobalPosition;
            }
        }

        private Node2D _parent;
        private Particles2D _particles;
        [Export] private int _maxPointCount = 20;

        [Export] private bool _hasParticles;
        [Export] private bool _usePlayerModulate;
    }

}