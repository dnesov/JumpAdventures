using System;
using Godot;
using GodotFmod;

namespace Jump.Entities
{
    [Tool]
    public class Grass : Node2D
    {
        [Export]
        public Texture GrassTexture
        {
            get => _grassTexture; set
            {
                _grassTexture = value;
                if (_grassSprite == null) return;
                _grassSprite.Texture = _grassTexture;
            }
        }
        public override void _Ready()
        {
            // Visible = false;

            _grassSprite = GetNode<Sprite>("Grass");
            _grassSprite.Texture = _grassTexture;

            if (!_interactive) return;
            _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
            _eventPlayer = GetNode<FmodEventPlayer2D>("%TouchSound");
            _target = _targetPath == null ? null : GetNode<Node2D>(_targetPath);
            _shaderMaterial = _grassSprite.Material as ShaderMaterial;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!Visible || !_interactive) return;
            if (_target == null) return;
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
            _shaderMaterial.SetShaderParam("offset", _speed);
        }

        private void PlaySwaySound()
        {
            // _fmodRuntime.PlayOneShot("event:/Environment/GrassTouch");
            _eventPlayer.Start();
        }

        private void ScreenEntered()
        {
            Visible = true;
            SetPhysicsProcess(true);
        }

        private void ScreenExited()
        {
            Visible = false;
            SetPhysicsProcess(false);
        }

        private ShaderMaterial _shaderMaterial;
        private float _distance;
        private Sprite _grassSprite;
        private FmodRuntime _fmodRuntime;
        private Node2D _target;
        private Texture _grassTexture;
        private FmodEventPlayer2D _eventPlayer;
        [Export] private NodePath _targetPath;
        [Export] private readonly float _distanceThreshold = 50f;
        [Export] private bool _interactive = true;

        private float _speed = 1f;
        private float _targetSpeed;
        private bool _inside;
    }
}
