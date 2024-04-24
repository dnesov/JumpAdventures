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
            _lastArea = area;
            var obstacle = _lastArea as IObstacle;
            obstacle?.PlayerEntered(_player);

            if (_lastArea is IInteractable interactable)
            {
                var message = interactable.GetInteractMessage();
                if (!string.IsNullOrEmpty(message))
                {
                    _player.GUI.DisplayMessage(message, 0.25f);
                }
            }
        }

        private void AreaExited(Area2D area)
        {
            _lastArea = area;
            var obstacle = _lastArea as IObstacle;
            obstacle?.PlayerExited(_player);

            if (_lastArea is IInteractable interactable)
            {
                _player.GUI.MessageUI.Hide();
            }
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (Input.IsActionJustPressed("interact"))
            {
                if (_lastArea != null && _lastArea is IInteractable interactable)
                {
                    interactable.OnInteract();
                }
            }
        }

        private void BodyEntered(Node body)
        {
            _lastBody = body;
            if (_lastBody is IObstacle obstacle) obstacle.PlayerEntered(_player);

            if (_lastBody is RigidBody2D rb)
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

            _lastBody = null;
        }

        [Export] private readonly float _interactImpulseStrength = 30f;
        [Export] private readonly Vector2 _physicsInteractMultiplier = new Vector2(1, 0.5f);
        private Player _player;

        private Node _lastBody;
        private Node _lastArea;
    }

}