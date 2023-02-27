using Godot;
using System;

namespace Jump.Entities
{
    public class PlayerInteractor : Area2D
    {
        public void EnableInteractions() => SetDeferred("monitoring", true);
        public void DisableInteractions() => SetDeferred("monitoring", false);

        // Called when the node enters the scene tree for the first time.
        public override async void _Ready()
        {
            GetNodes();
            ConnectSignals();
            await ToSignal(_player, "ready");
            SubscribeEvents();
        }

        private void GetNodes() => _player = GetParent<Player>();

        private void ConnectSignals()
        {
            Connect("area_entered", this, nameof(AreaEntered));
            Connect("area_exited", this, nameof(AreaExited));
            Connect("body_entered", this, nameof(BodyEntered));
            Connect("body_exited", this, nameof(BodyExited));
        }

        private void SubscribeEvents()
        {
            _player.HealthHandler.OnDeath += OnDeath;
            _player.OnAnyRespawn += OnRespawn;
        }

        private void OnDeath() => DisableInteractions();
        private void OnRespawn() => EnableInteractions();

        private void AreaEntered(Area2D area)
        {
            var obstacle = area as IObstacle;
            obstacle?.PlayerEntered(_player);
        }

        private void AreaExited(Area2D area)
        {
            var obstacle = area as IObstacle;
            obstacle?.PlayerExited(_player);
        }

        private void BodyEntered(Node body)
        {
            if (body is IObstacle obstacle) obstacle.PlayerEntered(_player);
            // if (body is ObstacleKinematicBody kinematicObstacle && _player.JustLanded) kinematicObstacle.OnPlayerLanded(_player);

            if (body is RigidBody2D rb)
            {
                if (rb.CollisionLayer != 16) return;
                var dir = _player.Velocity.Normalized();
                var impulse = dir * _physicsInteractMultiplier * _interactImpulseStrength;
                rb.ApplyCentralImpulse(impulse);
            }
        }

        private void BodyExited(Node body)
        {
            var obstacle = body as IObstacle;
            obstacle?.PlayerExited(_player);
        }

        [Export] private readonly float _interactImpulseStrength = 30f;
        [Export] private readonly Vector2 _physicsInteractMultiplier = new Vector2(1, 0.5f);
        private Player _player;
    }

}