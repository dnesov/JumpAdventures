using Jump.Extensions;

namespace Jump.Entities
{
    public abstract class ResetableObstacle : ObstacleBase
    {
        public override void _Ready()
        {
            base._Ready();
            _game = this.GetSingleton<Game>();
            SubscribeEvents();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        protected abstract void OnRestart();

        private void SubscribeEvents()
        {
            _game.OnLateRetry += OnRestart;
        }
        private void UnsubscribeEvents()
        {
            _game.OnLateRetry -= OnRestart;
        }

        private Game _game;
    }
}