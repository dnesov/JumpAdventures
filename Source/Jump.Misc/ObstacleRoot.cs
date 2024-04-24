using Jump.Entities;
using Godot;

namespace Jump.Misc
{
    [Tool]
    public class ObstacleRoot : Node2D
    {
        public bool IsDisabled => _disabled || !Visible;
        public override void _Ready()
        {
            base._Ready();

            if (IsDisabled)
            {
                DisableObstacles();
            }
        }
        public void EnableObstacles()
        {
            var children = GetChildren();
            foreach (var child in children)
            {
                if (child is ObstacleRoot obstacleRoot)
                {
                    obstacleRoot.EnableObstacles();
                }
                if (child is IObstacle obstacle)
                {
                    obstacle.Enable();
                }
            }
        }

        public void DisableObstacles()
        {
            var children = GetChildren();
            foreach (Node child in children)
            {
                if (child is ObstacleRoot obstacleRoot)
                {
                    obstacleRoot.DisableObstacles();
                }
                if (child is IObstacle obstacle)
                {
                    obstacle.Disable();
                }
            }
        }

        private void GetObstacles(Node node)
        {
            foreach (Node child in node.GetChildren())
            {

            }
        }

        [Export] private bool _disabled = true;
    }
}