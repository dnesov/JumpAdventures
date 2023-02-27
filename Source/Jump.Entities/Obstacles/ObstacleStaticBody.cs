using System;
using Godot;

namespace Jump.Entities
{
    public class ObstacleStaticBody : StaticBody2D, IObstacle
    {
        public Action<Player> OnPlayerEntered;
        public Action<Player> OnPlayerExited;

        public void PlayerEntered(Player player) => OnPlayerEntered?.Invoke(player);
        public void PlayerExited(Player player) => OnPlayerExited?.Invoke(player);
    }
}