using Godot;
using Jump.Utils;

namespace Jump.Unlocks
{
    public abstract class UnlockConditionBase : Resource
    {
        public abstract bool MeetsCondition();

        public void Register(ProgressHandler progressHandler)
        {
            this.progressHandler = progressHandler;
        }
        protected ProgressHandler progressHandler;
    }
}