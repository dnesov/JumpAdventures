using Godot;
using Jump.Extensions;

namespace Jump.Utils
{
    public class MenuBackgroundPicker : Node
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GetNodes();
            SubscribeEvents();

            SwitchBackground(_game.Data.LastWorldId);
        }

        public override void _ExitTree()
        {
            UnsubscribeEvents();
        }

        private void GetNodes()
        {
            _game = this.GetSingleton<Game>();
        }

        private void SubscribeEvents()
        {
            _game.OnWorldLoaded += SwitchBackground;
        }

        private void UnsubscribeEvents()
        {
            _game.OnWorldLoaded -= SwitchBackground;
        }

        private void SwitchBackground(string worldId)
        {
            if (_currentBackgroundId == worldId || worldId == string.Empty) return;
            var previousBgId = _currentBackgroundId;
            _currentBackgroundId = worldId;

            var previousBg = GetNode<TextureRect>(previousBgId);
            var currentBg = GetNode<TextureRect>(_currentBackgroundId);

            if (previousBgId != string.Empty)
                FadeOut(previousBg);

            FadeIn(currentBg);
        }

        private void FadeIn(TextureRect background)
        {
            var tween = CreateTween();
            tween.TweenCallback(background, "show");
            tween.TweenProperty(background, "self_modulate", new Color(1f, 1f, 1f, 1f), 0.3f);
        }

        private void FadeOut(TextureRect background)
        {
            var tween = CreateTween();
            tween.TweenProperty(background, "self_modulate", new Color(1f, 1f, 1f, 0f), 0.3f);
            tween.Chain().TweenCallback(background, "hide");
        }

        private string _currentBackgroundId = string.Empty;
        private Game _game;
    }
}