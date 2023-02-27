using Godot;
using GodotFmod;
using System;

namespace Jump.Entities
{
    [Tool]
    public class SinkingPolygonGround : PolygonGround
    {
        public override void _Ready()
        {
            base._Ready();

            var obstacleBody = collisionBody as ObstacleKinematicBody;
            obstacleBody.OnPlayerEntered += PlayerEntered;
            obstacleBody.OnPlayerExited += PlayerExited;

            _initialYPosition = GlobalPosition.y;

            _fmodRuntime = GetTree().Root.GetNode<FmodRuntime>("FmodRuntime");
            _tween = new Tween();
            AddChild(_tween);
        }

        private void PlayerEntered(Player player) => Sink();
        private void PlayerExited(Player player) => Resurface();

        private void Sink()
        {
            _fmodRuntime.PlayOneShot("event:/Splash");

            _tween.RemoveAll();
            Vector2 target = new Vector2(GlobalPosition.x, _initialYPosition - _sinkDepth);
            _tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
            _tween.InterpolateProperty(this, "position", GlobalPosition, target, _sinkTime, default, Tween.EaseType.Out);
            _tween.Start();
        }

        private void Resurface()
        {
            _tween.RemoveAll();
            Vector2 target = new Vector2(GlobalPosition.x, _initialYPosition);
            _tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
            _tween.InterpolateProperty(this, "position", GlobalPosition, target, _resurfaceTime, Tween.TransitionType.Elastic, Tween.EaseType.Out);
            _tween.Start();
        }


        private Tween _tween;
        private float _timer;
        private float _leaveTime = 0.2f;

        private float _initialYPosition;
        private bool _sinking;
        [Export] private float _sinkDepth = -200f;
        [Export] private float _resurfaceTime = 5f;
        [Export] private float _sinkTime = 3f;


        private FmodRuntime _fmodRuntime;
    }
}
