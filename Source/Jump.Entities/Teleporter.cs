using Godot;

namespace Jump.Entities
{
    public class Teleporter : ObstacleBase
    {
        protected override void OnPlayerEntered(Player player)
        {
            if (_teleportTarget == null) return;
            var target = GetNode(_teleportTarget);

            if (target is Teleporter teleporter)
            {
                player.GlobalPosition = teleporter.GlobalPosition;
                player.Camera.ResetPosition();
            }
        }

        protected override void OnPlayerExited(Player player)
        {

        }

        [Export] private NodePath _teleportTarget;
    }
}