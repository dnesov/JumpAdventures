using Godot;

namespace Jump.UI
{
    public class StatusUI : Control
    {
        [Export] private static StreamTexture _fullHeartTexture;
        [Export] private static StreamTexture _emptyHeartTexture;

        private HBoxContainer _healthContainer;
    }
}