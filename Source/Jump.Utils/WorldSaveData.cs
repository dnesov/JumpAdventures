using System.Collections.Generic;

namespace Jump.Utils
{
    public class WorldSaveData
    {
        public bool Unlocked { get => _unlocked; set => _unlocked = value; }
        public short FragmentsCollected { get => _fragmentsCollected; set => _fragmentsCollected = value; }
        public LevelSaveData[] LevelSaves { get => _levelSaves; set => _levelSaves = value; }

        public void CollectFragment()
        {
            FragmentsCollected++;
        }
        public void Unlock()
        {
            Unlocked = true;
        }

        public bool IsCompleted()
        {
            return GetCompletedLevelsAmount() == 20;
        }

        public LevelSaveData GetLevelSaveAt(int idx) => LevelSaves[idx];

        public LevelSaveData TryGetLevelSaveAt(int idx)
        {
            var levelSave = GetLevelSaveAt(idx);
            return levelSave == null ? CreateLevelSaveAt(idx) : levelSave;
        }

        public LevelSaveData CreateLevelSaveAt(int idx)
        {
            var levelSave = new LevelSaveData();
            LevelSaves[idx] = levelSave;

            return levelSave;
        }

        public int GetCompletedLevelsAmount()
        {
            int result = 0;
            foreach (var levelSave in _levelSaves)
            {
                if (levelSave == null) continue;
                if (levelSave.Completed) result++;
            }

            return result;
        }

        private short _fragmentsCollected = 0;
        private bool _unlocked = false;
        private LevelSaveData[] _levelSaves = new LevelSaveData[32];
    }

    public class LevelSaveData
    {
        public bool Completed { get => _completed; set => _completed = value; }
        public int Attempts { get => _attempts; set => _attempts = value; }
        public HashSet<int> CollectedCollectibles { get => _collectedCollectibles; set => _collectedCollectibles = value; }

        public void Complete()
        {
            Completed = true;
        }

        public void CollectCollectible(int idx)
        {
            CollectedCollectibles.Add(idx);
        }

        private bool _completed;
        private HashSet<int> _collectedCollectibles = new HashSet<int>();
        private int _attempts = 1;
    }
}