using Godot;
using System;

namespace Jump.Entities
{
    public abstract class Collectible : Area2D, IObstacle
    {
        [Export] public int Id { get; set; }
        public Action<int> OnCollected;

        protected abstract void Collected(Player player);
        protected abstract bool IsCollected();

        public void PlayerEntered(Player player)
        {
            if (IsCollected()) return;
            Collected(player);
            OnCollected?.Invoke(Id);
        }

        public void PlayerExited(Player player) { }

        protected Area2D area;
    }
}