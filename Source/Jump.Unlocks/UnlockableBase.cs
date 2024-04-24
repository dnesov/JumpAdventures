using System.Collections.Generic;
using System.Linq;
using Godot;
using Jump.Utils;

namespace Jump.Unlocks
{
    public abstract class UnlockableBase : Resource
    {
        public ProgressHandler ProgressHandler => progressHandler;
        [Export] public string Title { get; set; }
        [Export] protected string description { get; set; }
        [Export] public bool IsTranslationString, IsFormatted;
        public abstract string FormattedDescription { get; }
        [Export] public string[] Formats { get; set; } = new string[] { };
        [Export] public Texture Icon { get; set; }
        [Export(PropertyHint.ColorNoAlpha)] public Color IconModulate { get; set; } = new Color(1f, 1f, 1f);
        public string EntryId => _entryId;

        public bool CanUnlock()
        {
            if (_conditions == null || _conditions.Count == 0) return true;
            foreach (UnlockConditionBase condition in _conditions)
            {
                if (!condition.MeetsCondition()) return false;
            }

            return true;
        }

        public void Register(ProgressHandler progressHandler, string entryId = "")
        {
            this.progressHandler = progressHandler;
            _entryId = entryId;
            foreach (var condition in _conditions)
            {
                if (condition == null) continue;
                condition.Register(progressHandler);
            }
        }

        public T GetCondition<T>() where T : UnlockConditionBase
        {
            return _conditions.OfType<T>().FirstOrDefault();
        }

        public bool HasCondition<T>() where T : UnlockConditionBase
        {
            return _conditions.Exists(i => i.GetType() == typeof(T));
        }

        protected ProgressHandler progressHandler;
        [Export] private List<UnlockConditionBase> _conditions = new List<UnlockConditionBase>();
        private string _entryId;
    }
}