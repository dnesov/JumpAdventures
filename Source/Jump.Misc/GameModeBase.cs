using System;
using Jump.Entities;
using Jump.Utils;

namespace Jump.Misc
{
    public abstract class GameModeBase
    {
        public Action OnEssenceChanged;
        public Action OnEssenceCollected;
        public int Essence
        {
            get => _essence;
            set
            {
                _essence = value;
                OnEssenceChanged?.Invoke();
            }
        }

        public void Register(Game game, ProgressHandler progressHandler)
        {
            this.game = game;
            this.progressHandler = progressHandler;
        }

        public void RegisterPlayer(Player player)
        {
            this.player = player;
        }

        public abstract string Name { get; }
        public abstract string ObstacleRootName { get; }
        public abstract short MaxHearts { get; }
        public abstract bool EnableCheats { get; }
        public abstract bool ShouldUseTimer { get; set; }

        public abstract void OnWin();
        public abstract void OnRetry();
        public virtual void OnLateRetry() { }
        public abstract void OnGameOver();
        public abstract void OnStoppedPlaying();
        public abstract void EssenceCollected();

        protected Game game;
        protected ProgressHandler progressHandler;
        protected Player player;

        private int _essence;
    }
}