using Godot;
using System;

namespace Jump.UI
{
    public class MessageUI : UIElement<MessageUIData>
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _label = GetNode<RichTextLabel>("%MessageLabel");
        }

        protected override void OnDisplay()
        {
            var tween = CreateTween();
            tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 1), _duration);
        }

        protected override void OnHide()
        {
            var tween = CreateTween();
            tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0f), _duration);
        }

        protected override void OnUpdateElements(MessageUIData data)
        {
            _duration = data.Duration;
            _label.BbcodeText = data.Message;
        }

        private float _duration;
        private RichTextLabel _label;
    }

    public class MessageUIData
    {
        public string Message;
        public float Duration = 0.3f;
    }
}
