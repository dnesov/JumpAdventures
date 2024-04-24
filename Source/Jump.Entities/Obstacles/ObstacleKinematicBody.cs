using Godot;
using System;

namespace Jump.Entities
{
    public class ObstacleKinematicBody : KinematicBody2D, IObstacle
    {
        public Action<Player> OnPlayerEntered;
        public Action<Player> OnPlayerExited;
        public Action<Player> OnPlayerLanded;

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
        public void PlayerLanded(Player player) => OnPlayerLanded?.Invoke(player);
    }
}
