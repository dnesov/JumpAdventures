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

    public void Enable()
    {
        throw new System.NotImplementedException();
    }

    public void Disable()
    {
        throw new System.NotImplementedException();
    }

    [Export] private float _friction;
}
