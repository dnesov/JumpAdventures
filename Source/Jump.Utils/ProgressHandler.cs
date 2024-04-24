using System;
using System.Linq;
using Godot;
using Jump.Extensions;
using Jump.Utils.SaveData;

namespace Jump.Utils
{
    public class ProgressHandler : Node
    {
        public int Essence { get => _essence; set => _essence = value; }
        public int GlobalFragments { get => _globalFragments; set => _globalFragments = value; }
        public int MaxFragments => _maxFragments;
        public int Experience { get => _experience; set => _experience = value; }
        public int Jumps { get => _jumps; set => _jumps = value; }
        public int TotalAttempts { get => _totalAttempts; set => _totalAttempts = value; }
        public int TotalLevelsComplete => _totalLevelsComplete;
        public int MaxLevels => _maxLevels;

        public TimeSpan PlayTime => TimeSpan.FromSeconds(_playTimeSeconds);

        public Action OnFragmentCollected;

        public override void _Ready()
        {
            base._Ready();
            GetNodes();
            SubscribeEvents();

            _saveDataSerializer = new SaveDataSerializer();
            Load();

            PauseMode = PauseModeEnum.Process;

            GetProgressForAllWorlds();
        }

        public WorldProgressData GetWorldProgress(string forWorldId)
        {
            WorldProgressData result;
            var data = _game.WorldSaveHandler.TryLoadData(forWorldId);

            int levelsCompleted = data.GetCompletedLevelsAmount();

            result = new WorldProgressData()
            {
                LevelsCompleted = levelsCompleted,
                PercentageComplete = levelsCompleted / 20f,
                IsComplete = levelsCompleted == 20
            };

            return result;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            _playTimeSeconds += delta;
        }

        /// <summary>
        /// Collects a fragment for the current world.
        /// </summary>
        public void CollectFragment() => CollectFragmentFor(_game.CurrentWorld.UniqueId);

        /// <summary>
        /// Collects a fragment for a specified World.
        /// </summary>
        /// <param name="worldId"></param>
        public void CollectFragmentFor(string worldId)
        {
            _globalFragments++;
            var data = _game.WorldSaveHandler.TryLoadData(worldId);
            data.CollectFragment();
            _game.WorldSaveHandler.TrySaveData(worldId, data);

            OnFragmentCollected?.Invoke();
        }

        private void GetNodes()
        {
            _game = this.GetSingleton<Game>();
        }
        private void SubscribeEvents()
        {
            _game.OnSave += Save;
        }

        private void Save()
        {
            var saveData = new GlobalProgressSaveData()
            {
                Essence = _essence,
                Fragments = _globalFragments,
                Experience = _experience,
                Attempts = _totalAttempts,
                Jumps = _jumps,
                PlayTime = _playTimeSeconds
            };

            _saveDataSerializer.SerializeCeras(saveData);

            GetProgressForAllWorlds();
        }

        private void Load()
        {
            var saveData = _saveDataSerializer.DeserializeCeras<GlobalProgressSaveData>(_saveFileName);

            _essence = saveData.Essence;
            _globalFragments = saveData.Fragments;
            _experience = saveData.Experience;
            _totalAttempts = saveData.Attempts;
            _jumps = saveData.Jumps;
            _playTimeSeconds = saveData.PlayTime;
        }

        private void GetProgressForAllWorlds()
        {
            _totalLevelsComplete = 0;

            foreach (var worldId in Constants.WorldIds.AsArray)
            {
                var progress = GetWorldProgress(worldId);

                _totalLevelsComplete += progress.LevelsCompleted;
            }

            if (_maxFragments > 0 || _maxLevels > 0) return;

            foreach (var world in _game.WorldpackScanner.LoadedWorlds)
            {
                if (world.Value.Hidden || !world.Value.Playable) continue;

                _maxFragments += 6;
                var levelCount = world.Value.Levels.Count();
                _maxLevels += levelCount;
            }
        }

        private Game _game;
        private SaveDataSerializer _saveDataSerializer;
        private int _essence;
        private int _globalFragments;
        private int _experience;
        private int _jumps;
        private int _totalAttempts;
        private float _playTimeSeconds;
        private int _totalLevelsComplete;

        private int _maxFragments = 0;
        private int _maxLevels = 0;

        private readonly string _saveFileName = "global_progress.jasv";
    }
}