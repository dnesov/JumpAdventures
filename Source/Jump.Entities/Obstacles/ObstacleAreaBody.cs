using System;
using Godot;

namespace Jump.Entities
{
    public class ObstacleAreaBody : Area2D, IObstacle
    {
        public Action<Player> OnPlayerEntered;
        public Action<Player> OnPlayerExited;

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public void Enable()
        {
            throw new NotImplementedException();
        }

        public void PlayerEntered(Player player) => OnPlayerEntered?.Invoke(player);
        public void PlayerExited(Player player) => OnPlayerExited?.Invoke(player);
    }
}