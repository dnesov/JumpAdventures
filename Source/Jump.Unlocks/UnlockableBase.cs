using System.Collections.Generic;
using System.Linq;
using Godot;
using Jump.Utils;

namespace Jump.Unlocks
{
    public abstract class UnlockableBase : Resource
    {
        public ProgressHandler ProgressHandler => progressHandler;
        [Export] protected string description { get; set; }

        [Export] public bool IsTranslationString, IsFormatted;

        public abstract string FormattedDescription { get; }
        [Export] public string[] Formats { get; set; } = new string[] { };

        public bool CanUnlock()
        {
            if (_conditions == null || _conditions.Count == 0) return true;
            foreach (UnlockConditionBase condition in _conditions)
            {
                if (!condition.MeetsCondition()) return false;
            }

            return true;
        }

        public void Register(ProgressHandler progressHandler)
        {
            this.progressHandler = progressHandler;
            foreach (var condition in _conditions)
            {
                if (condition == null) continue;
                condition.Register(progressHandler);
            }
        }

        public T GetCondition<T>() where T : UnlockConditionBase
        {
            return _conditions.OfType<T>().First();
        }

        protected ProgressHandler progressHandler;
        [Export] private List<UnlockConditionBase> _conditions = new List<UnlockConditionBase>();
    }
}