using Godot;
using Jump.Extensions;

namespace Jump.Misc
{
    public class LevelRoot : Node2D
    {
        public int MaxEssence;
        public override void _Ready()
        {
            base._Ready();
            GetNodes();
            ToggleGameModeRoot();
        }

        private void GetNodes()
        {
            _game = this.GetSingleton<Game>();
        }

        private void ToggleGameModeRoot()
        {
            var rootName = _game.CurrentGameMode.ObstacleRootName;
            if (rootName.Empty() || rootName == null) return;

            ObstacleRoot root = GetNodeOrNull<ObstacleRoot>(rootName);
            if (root == null) return;

            if (root.IsDisabled)
            {
                root.EnableObstacles();
            }
        }

        private Game _game;
    }
}