using Godot;

namespace Jump.Entities
{
    [Tool]
    public class StickyPolygonGround : PolygonGround
    {
        public override void _Ready()
        {
            base._Ready();
            var obstacleBody = collisionBody as ObstacleKinematicBody;
            obstacleBody.OnPlayerEntered += PlayerEntered;
            obstacleBody.OnPlayerExited += PlayerExited;
        }

        private void PlayerEntered(Player player) => player.BeginStick();

        private void PlayerExited(Player player) => player.EndStick();
    }
}