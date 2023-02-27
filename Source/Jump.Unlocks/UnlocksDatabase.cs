using System;
using System.Collections.Generic;
using Godot;
using Jump.Utils;

namespace Jump.Unlocks
{
    public class UnlocksDatabase : Node
    {
        public Action<UnlockableBase> OnUnlockableJustUnlocked;
        public override void _Ready()
        {
            base._Ready();
            GetNodes();
            RegisterUnlockables();

            _game.OnQuit += OnQuit;

            var loadedUnlocks = _serializer.SaveExists()
            ? _unlocks = _serializer.Deserialize()
            : null;

            if (loadedUnlocks == null) CreateUnlocks(); return;
        }

        public UnlockableBase GetUnlockable(string byStringId)
        {
            if (!_entries.ContainsKey(byStringId)) return null;
            return _entries[byStringId];
        }

        public bool TryUnlock(string unlockableId)
        {
            if (!IsIdValid(unlockableId)) return false;
            var unlockable = _entries[unlockableId];

            if (unlockable.CanUnlock())
            {
                _unlocks[unlockableId] = true;
                return true;
            }
            return false;
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
            if (_entries == null) return false;

            return _entries[unlockableId].CanUnlock();
        }

        private void GetNodes()
        {
            _game = GetTree().Root.GetNode<Game>("Game");
            _serializer = new UnlocksSerializer();
            _progressHandler = GetTree().Root.GetNode<ProgressHandler>("ProgressHandler");
        }

        private bool GetFromUnlocks(string unlockableId)
        {
            if (_unlocks.ContainsKey(unlockableId)) return _unlocks[unlockableId];
            var unlockable = GetUnlockable(unlockableId);
            _unlocks.Add(unlockableId, false);

            return _unlocks[unlockableId];
        }

        private bool IsIdValid(string id) => _entries.ContainsKey(id);

        private void RegisterUnlockables()
        {
            foreach (var entry in _entries)
            {
                entry.Value.Register(_progressHandler);
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
            foreach (var entry in _entries)
            {
                _unlocks.Add(entry.Key, entry.Value.CanUnlock());
            }
        }

        // TODO: should be a responsibility of a separate class.
        // private ProgressData GetProgressData()
        // {
        //     var dataHandler = _game.DataHandler;
        //     var result = new ProgressData
        //     {
        //         Collectibles = new CollectibleProgressData
        //         {
        //             Essence = dataHandler.Data.SaveData.Essence,
        //             Fragments = 0,
        //         },
        //     };

        //     return result;
        // }
        private Game _game;
        private UnlocksSerializer _serializer;
        private ProgressHandler _progressHandler;
        [Export] private Dictionary<string, UnlockableBase> _entries;
        private Dictionary<string, bool> _unlocks;
    }
}