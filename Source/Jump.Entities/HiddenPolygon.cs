using Godot;

namespace Jump.Entities
{
    [Tool]
    public class HiddenPolygon : PolygonGround
    {
        public override void _Ready()
        {
            base._Ready();
            var obstacleBody = collisionBody as ObstacleAreaBody;
            obstacleBody.OnPlayerEntered += PlayerEntered;
            obstacleBody.OnPlayerExited += PlayerExited;

            _tween = new Tween();
            AddChild(_tween);

            Hide();
        }

        private void PlayerEntered(Player player) => Reveal();
        private void PlayerExited(Player player) => Hide();

        private void Reveal()
        {
            _tween.StopAll();
            var target = new Color(Modulate.r, Modulate.g, Modulate.b, _alpha);
            _tween.InterpolateProperty(this, "modulate", Modulate, target, _tweenDuration);
            _tween.Start();
        }

        private new void Hide()
        {
            _tween.StopAll();
            var target = new Color(Modulate.r, Modulate.g, Modulate.b, 1f);
            _tween.InterpolateProperty(this, "modulate", Modulate, target, _tweenDuration);
            _tween.Start();
        }

        [Export] private float _tweenDuration = 0.25f;
        [Export] private float _alpha = 0.8f;
        private Tween _tween;
    }
}