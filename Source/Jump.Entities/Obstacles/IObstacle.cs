using Godot;

namespace Jump.Entities
{
    public interface IObstacle
    {
        void PlayerEntered(Player player);
        void PlayerExited(Player player);
        void Enable();
        void Disable();
    }
}