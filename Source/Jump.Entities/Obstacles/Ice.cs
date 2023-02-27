using Godot;
using Jump.Entities;

public class Ice : Area2D, IObstacle
{
    public void PlayerEntered(Player player)
    {
        player.Friction = _friction;
    }

    public void PlayerExited(Player player)
    {
        player.ResetFriction();
    }

    [Export] private float _friction;
}
