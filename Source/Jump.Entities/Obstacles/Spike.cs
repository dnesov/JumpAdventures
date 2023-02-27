using Godot;
using Jump.Entities;
using System;
namespace Jump.Entities
{
    public class Spike : ObstacleBase
    {
        protected override void OnPlayerEntered(Player player)
        {
            player.HealthHandler.Damage(1);
            if (_player != null) return;
            _player = player;
        }

        protected override void OnPlayerExited(Player player)
        {

        }

        public override void _PhysicsProcess(float delta)
        {
            if (_player == null) return;
            if (!playerInside) return;
            _player.HealthHandler.Damage(1, 1f);
        }

        private Player _player;
    }

}