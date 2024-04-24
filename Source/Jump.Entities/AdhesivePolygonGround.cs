using Godot;

namespace Jump.Entities
{
    [Tool]
    public class AdhesivePolygonGround : PolygonGround
    {
        public override void _Ready()
        {
            base._Ready();

            _obstacleBody = collisionBody as ObstacleKinematicBody;

            SubscribeEvents();
        }

        public override void _ExitTree()
        {
            UnsubscribeEvents();
        }

        private void PlayerEntered(Player player) => player.BeginAdhesion();
        private void PlayerExited(Player player) => player.EndAdhesion();

        private void SubscribeEvents()
        {
            _obstacleBody.OnPlayerEntered += PlayerEntered;
            _obstacleBody.OnPlayerExited += PlayerExited;
        }

        private void UnsubscribeEvents()
        {
            _obstacleBody.OnPlayerEntered -= PlayerEntered;
            _obstacleBody.OnPlayerExited -= PlayerExited;
        }

        private ObstacleKinematicBody _obstacleBody;
    }
}