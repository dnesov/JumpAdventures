using Godot;
using Jump.Misc;
using System;

namespace Jump.Entities
{
    public abstract class Collectible : Area2D, IObstacle
    {
        public override void _Ready()
        {
            base._Ready();
            root = GetOwner<LevelRoot>();
        }
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

        public void Enable()
        {
            Monitoring = true;
            Visible = true;
        }

        public void Disable()
        {
            Monitoring = false;
            Visible = false;
        }

        protected LevelRoot root;
    }
}