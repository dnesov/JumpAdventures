using Godot;
using Jump.Entities;
using Jump.Extensions;

namespace Jump.Triggers
{
    public class RetryTrigger : BaseTrigger
    {
        public override void _Ready()
        {
            base._Ready();
            _game = this.GetSingleton<Game>();
        }
        protected override void OnEntered(Player player) => _game.TryRetry(_shouldAddAttempt);
        protected override void OnExited(Player player)
        {

        }
        private Game _game;
        [Export] private bool _shouldAddAttempt = true;
    }
}