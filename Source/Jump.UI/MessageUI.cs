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

        public void DisplayMessage(string message, float duration)
        {
            var data = new MessageUIData()
            {
                Message = $"[center]{message}[/center]",
                Duration = duration

            };

            UpdateElements(data);

            _lastMessage = data;

            Display();
        }

        protected override void OnDisplay()
        {
            if (_tweener is not null && _tweener.IsRunning())
            {
                _tweener.Kill();
            }

            _tweener = CreateTween();
            _tweener.TweenProperty(this, "modulate", new Color(1, 1, 1, 1), _duration);
        }

        protected override void OnHide()
        {
            if (_tweener is not null && _tweener.IsRunning())
            {
                _tweener.Kill();
            }

            _tweener = CreateTween();
            _tweener.TweenProperty(this, "modulate", new Color(1, 1, 1, 0f), _duration);
        }

        protected override void OnUpdateElements(MessageUIData data)
        {
            _duration = data.Duration;
            _label.BbcodeText = data.Message;
        }

        private float _duration;
        private RichTextLabel _label;
        private MessageUIData _lastMessage;

        private SceneTreeTween _tweener;
    }

    public class MessageUIData
    {
        public string Message;
        public float Duration = 0.3f;
    }
}
