using Godot;
using Jump.Entities;

using Jump.Extensions;

namespace Jump.Triggers
{
    public class MessageTrigger : BaseTrigger
    {
        protected override void OnEntered(Player player)
        {
            var game = this.GetSingleton<Game>();
            var suffix = _isInputMethodDependent ? game.GetTranslationInputSuffix() : string.Empty;
            var message = _isTranslationString ? Tr($"{_message}{suffix}") : _message;

            player.GUI.DisplayMessage(message, _duration);
        }

        protected override void OnExited(Player player) => player.GUI.MessageUI.Hide();

        [Export] private float _duration = 0.3f;
        [Export(PropertyHint.MultilineText)] private string _message;
        [Export] private bool _isTranslationString;
        [Export] private bool _isInputMethodDependent;
    }
}