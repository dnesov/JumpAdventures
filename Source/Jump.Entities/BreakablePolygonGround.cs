using Godot;
using Jump.Extensions;

namespace Jump.Entities
{
    [Tool]
    public class BreakablePolygonGround : PolygonGround, IRestartable
    {
        public override void _Ready()
        {
            base._Ready();
            var obstacleBody = collisionBody as ObstacleStaticBody;
            obstacleBody.OnPlayerLanded += PlayerLanded;

            _game = this.GetSingleton<Game>();
            _game.OnLateRetry += Restart;
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            var obstacleBody = collisionBody as ObstacleStaticBody;
            obstacleBody.OnPlayerLanded -= PlayerLanded;

            _game.OnLateRetry -= Restart;
        }
        private void PlayerExited(Player player)
        {
            _player = null;
        }
        private async void PlayerLanded(Player player)
        {
            if (_timer == null)
            {
                _timer = GetTree().CreateTimer(_timeToDestroy);
                await _timer.TimeOut();
            }

            Destroy();
        }

        private void Destroy()
        {
            Hide();
            collisionPolygon.Disabled = true;
            _timer = null;
        }

        public void Restart()
        {
            _timer = null;
            Show();
            collisionPolygon.Disabled = false;
        }

        private Player _player;
        private Game _game;
        [Export] private float _timeToDestroy = 1.0f;
        private SceneTreeTimer _timer;
    }
}