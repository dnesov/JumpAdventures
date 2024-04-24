using Godot;
using Jump.Extensions;

namespace Jump.Utils
{
    public class ControllerRumbler : Node
    {
        public override void _Ready()
        {
            base._Ready();
            _game = this.GetSingleton<Game>();
        }
        public void RumbleWeak(float strength, float duration = 0.0f)
        {
            if (_game.LastInputMethod != InputMethod.Controller) return;

            var mul = _game.Data.Settings.AccessibilitySettings.ControllerVibration;
            Input.StartJoyVibration(0, Mathf.Clamp(strength * mul, 0.0f, 1.0f), 0.0f, duration);
        }

        public void RumbleStrong(float strength, float duration = 0.0f)
        {
            if (_game.LastInputMethod != InputMethod.Controller) return;

            var mul = _game.Data.Settings.AccessibilitySettings.ControllerVibration;
            Input.StartJoyVibration(0, 0.0f, Mathf.Clamp(strength * mul, 0.0f, 1.0f), duration);
        }

        private Game _game;
    }
}