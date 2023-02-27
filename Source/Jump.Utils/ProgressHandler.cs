using Godot;

namespace Jump.Utils
{
    public class ProgressHandler : Node
    {
        public override void _Ready()
        {
            base._Ready();
            GetNodes();
        }
        private void GetNodes()
        {
            _game = GetTree().Root.GetNode<Game>("Game");
        }

        public WorldProgressData GetWorldProgress(string forWorldId)
        {
            WorldProgressData result;
            var data = _game.WorldSaveHandler.TryLoadData(forWorldId);

            int levelsCompleted = data.GetCompletedLevelsAmount();

            result = new WorldProgressData()
            {
                LevelsCompleted = levelsCompleted,
                PercentageComplete = (float)levelsCompleted / 20f
            };

            return result;
        }

        public CollectibleProgressData GetCollectibleProgress()
        {
            CollectibleProgressData result;
            var data = _game.DataHandler.Data;

            result = new CollectibleProgressData()
            {
                Essence = data.SaveData.Essence,
                Fragments = 0
            };
            return result;
        }

        private Game _game;
    }
}