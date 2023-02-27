using Godot;
using Jump.Entities;
using System;

namespace Jump.Triggers
{
    public class MessageTrigger : BaseTrigger
    {
        protected override void OnEntered(Player player)
        {
            var message = _isTranslationString ? Tr(_message) : _message;
            player.GUI.MessageUI.UpdateElements(new UI.MessageUIData()
            {
                Message = $"[center]{message}[/center]",
                Duration = _duration
            });
            player.GUI.MessageUI.Display();
        }

        protected override void OnExited(Player player) => player.GUI.MessageUI.Hide();

        [Export] private float _duration = 0.3f;
        [Export(PropertyHint.MultilineText)] private string _message;
        [Export] private bool _isTranslationString;
    }
}