using System;
using System.Collections.Generic;
using Godot;
using Jump.Extensions;
using Jump.Utils;
using YamlDotNet.Serialization;

namespace Jump.Unlocks
{
    [Tool]
    public class UnlocksDatabase : Node
    {
        public Action<UnlockableBase> OnUnlockableJustUnlocked;
        public UnlockEntries Entries => _entries;
        public IReadOnlyDictionary<string, bool> Unlocks => _unlocks;

        public override void _Ready()
        {
            base._Ready();
            GetNodes();
            LoadUnlockEntries();
            RegisterUnlockables();

            _game.OnQuit += OnQuit;

            var loadedUnlocks = _serializer.SaveExists()
            ? _unlocks = _serializer.Deserialize()
            : null;

            if (loadedUnlocks == null) CreateUnlocks(); return;
        }

        public UnlockableBase GetUnlockable(string byStringId)
        {
            if (byStringId == null) return null;
            if (!_entries.ContainsId(byStringId) || string.IsNullOrEmpty(byStringId)) return null;
            return _entries.GetUnlockableBy(byStringId);
        }

        public bool TryGetUnlockable(string byStringId, out UnlockableBase unlockable)
        {
            unlockable = null;
            if (!_entries.ContainsId(byStringId)) return false;
            unlockable = _entries.GetUnlockableBy(byStringId);
            return true;
        }

        public bool TryUnlock(string unlockableId)
        {
            if (!IsIdValid(unlockableId)) return false;
            var unlockable = _entries.GetUnlockableBy(unlockableId);

            if (unlockable.CanUnlock())
            {
                _unlocks[unlockableId] = true;
                OnUnlockableJustUnlocked?.Invoke(unlockable);
                return true;
            }
            return false;
        }

        public void TryUnlockWorlds()
        {
            foreach (var unlockable in GetUnlockableUnlocks())
            {
                if (!unlockable.HasCondition<WorldCompleteCondition>()) continue;
                TryUnlock(unlockable.EntryId);
            }
        }

        public bool IsUnlocked(string unlockableId)
        {
            if (!IsIdValid(unlockableId)) return true;
            if (_unlocks == null) return false;
            return GetFromUnlocks(unlockableId);
        }

        public bool CanUnlock(string unlockableId)
        {
            if (!IsIdValid(unlockableId)) return true;
            if (Entries == null) return false;

            return _entries.GetUnlockableBy(unlockableId).CanUnlock();
        }

        public List<UnlockableBase> GetUnlockableUnlocks()
        {
            var result = new List<UnlockableBase>(_entries.Unlockables.Count);

            foreach (var entry in _entries.Unlockables)
            {
                if (CanUnlock(entry.Key) && !IsUnlocked(entry.Key)) result.Add(entry.Value);
            }

            return result;
        }

        public bool IsIdValid(string id) => _entries.ContainsId(id);

        private void GetNodes()
        {
            _game = this.GetSingleton<Game>();
            _serializer = new UnlocksSerializer();
            _progressHandler = this.GetSingleton<ProgressHandler>();
        }

        private bool GetFromUnlocks(string unlockableId)
        {
            if (_unlocks.ContainsKey(unlockableId)) return _unlocks[unlockableId];
            var unlockable = GetUnlockable(unlockableId);
            _unlocks.Add(unlockableId, false);

            return _unlocks[unlockableId];
        }

        private void RegisterUnlockables()
        {
            foreach (var entry in _entries.Unlockables)
            {
                entry.Value.Register(_progressHandler, entry.Key);
            }
        }

        private void OnQuit()
        {
            if (_unlocks == null) return;
            _serializer.Serialize(_unlocks);
        }

        private void CreateUnlocks()
        {
            _unlocks = new Dictionary<string, bool>();
            foreach (var entry in _entries.Unlockables)
            {
                _unlocks.Add(entry.Key, entry.Value.CanUnlock());
            }
        }

        private void LoadUnlockEntries()
        {
            string path = "res://Assets/Data/unlock_entries.yaml";

            var file = new File();
            file.Open(path, File.ModeFlags.Read);
            string yaml = file.GetAsText();

            var deserializer = new DeserializerBuilder().Build();

            var entries = deserializer.Deserialize<UnlockEntries>(yaml);
            _entries = entries;

            _entries.LoadUnlockables();
        }

        private Game _game;
        private UnlocksSerializer _serializer;
        private ProgressHandler _progressHandler;
        private UnlockEntries _entries;
        private Dictionary<string, bool> _unlocks;
    }
}