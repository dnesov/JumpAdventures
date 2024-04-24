using Godot;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Jump.Unlocks
{
    public class UnlockEntries
    {
        public Dictionary<string, string> UnlockIds { get; set; }
        [YamlIgnore] public Dictionary<string, UnlockableBase> Unlockables { get; set; }

        public void LoadUnlockables()
        {
            Unlockables = GetUnlockables();
        }

        public bool ContainsId(string id) => UnlockIds.ContainsKey(id);
        public UnlockableBase GetUnlockableBy(string id)
        {
            var unlockable = GD.Load<UnlockableBase>(UnlockIds[id]);
            return unlockable;
        }

        private Dictionary<string, UnlockableBase> GetUnlockables()
        {
            var result = new Dictionary<string, UnlockableBase>();

            foreach (var id in UnlockIds)
            {
                var unlockable = GetUnlockableBy(id.Key);
                result.Add(id.Key, unlockable);
            }

            return result;
        }
    }
}