using Godot;
using GodotFmod;
using Jump.Extensions;
using System;

namespace Jump.Entities
{
    [Tool]
    public class SinkingPolygonGround : PolygonGround, IRestartable
    {
        public override void _Ready()
        {
            base._Ready();

            _initialYPosition = GlobalPosition.y;

            _fmodRuntime = this.GetSingleton<FmodRuntime>();
            _game = this.GetSingleton<Game>();

            _tween = new Tween();
            AddChild(_tween);

            SubscribeEvents();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        public async void Restart()
        {
            await ToSignal(GetTree(), "idle_frame"); // Wait a frame before reseting the position, to avoid launching the player on respawn.
            _tween.RemoveAll();
            GlobalPosition = new Vector2(GlobalPosition.x, _initialYPosition);
            _sinkAccumulator = 0f;
        }

        private void SubscribeEvents()
        {
            var obstacleBody = collisionBody as ObstacleKinematicBody;
            obstacleBody.OnPlayerEntered += PlayerEntered;
            obstacleBody.OnPlayerExited += PlayerExited;

            _game.OnLateRetry += Restart;
        }
        private void UnsubscribeEvents()
        {
            var obstacleBody = collisionBody as ObstacleKinematicBody;
            obstacleBody.OnPlayerEntered -= PlayerEntered;
            obstacleBody.OnPlayerExited -= PlayerExited;
            _game.OnLateRetry -= Restart;
        }

        private void PlayerEntered(Player player) => Sink();
        private void PlayerExited(Player player) => Resurface();

        private void Sink()
        {
            _fmodRuntime.PlayOneShot("event:/Splash");

            _sinkAccumulator += _sinkPerLanding;

            _tween.RemoveAll();
            Vector2 target = new Vector2(GlobalPosition.x, _initialYPosition - _sinkDepth - _sinkAccumulator);
            _tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
            _tween.InterpolateProperty(this, "position", GlobalPosition, target, _sinkTime, default, Tween.EaseType.Out);
            _tween.Start();
        }

        private void Resurface()
        {
            _tween.RemoveAll();
            Vector2 target = new Vector2(GlobalPosition.x, _initialYPosition - _sinkAccumulator);
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
        [Export] private float _sinkPerLanding = -15f;
        private float _sinkAccumulator;
        [Export] private float _resurfaceTime = 5f;
        [Export] private float _sinkTime = 3f;


        private FmodRuntime _fmodRuntime;
        private Game _game;
    }
}
