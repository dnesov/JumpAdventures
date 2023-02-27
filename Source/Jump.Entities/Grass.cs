using System;
using Godot;
using GodotFmod;

namespace Jump.Entities
{
    public class Grass : Node2D
    {
        public override void _Ready()
        {
            Visible = false;
            if (!_interactive) return;
            _target = GetNode<Node2D>(_targetPath);
            _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
            _grassSprite = GetNode<Sprite>("%Grass");
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!Visible || !_interactive) return;
            _distance = GlobalPosition.DistanceTo(_target.GlobalPosition);

            if (_distance < _distanceThreshold)
            {
                StartSway();
            }
            else
            {
                StopSway();
            }

            ProcessSway(delta);
        }

        private void StartSway()
        {
            if (_inside) return;
            _targetSpeed = 10f;
            PlaySwaySound();
            _inside = true;
        }

        private void StopSway()
        {
            if (!_inside) return;
            _targetSpeed = 1f;
            _inside = false;
        }

        private void ProcessSway(float delta)
        {
            _speed = Mathf.Lerp(_speed, _targetSpeed, delta * 2);
            var material = _grassSprite.Material as ShaderMaterial;
            material.SetShaderParam("offset", _speed);
        }

        private void PlaySwaySound()
        {
            _fmodRuntime.PlayOneShot("event:/Environment/GrassTouch");
        }

        private void ScreenEntered() => Visible = true;

        private void ScreenExited() => Visible = false;

        private float _distance;
        private Sprite _grassSprite;
        private FmodRuntime _fmodRuntime;
        private Node2D _target;
        [Export] private NodePath _targetPath;
        [Export] private readonly float _distanceThreshold = 50f;
        [Export] private bool _interactive = true;

        private float _speed = 1f;
        private float _targetSpeed;
        private bool _inside;
    }
}
