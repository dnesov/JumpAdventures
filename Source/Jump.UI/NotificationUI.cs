using Godot;
using Jump.Utils;
using System;

namespace Jump.UI
{
    public class NotificationUI : UIElement<Notification>
    {
        protected override void OnDisplay()
        {
            GetNodes();
            PlayDisplayAnimation();
        }
        protected override void OnHide() => PlayHideAnimation();

        protected override void OnUpdateElements(Notification data)
        {
            _titleLabel.Text = data.Title;
            _contentsLabel.Text = data.Contents;
            _icon.Texture = data.Icon;
            _icon.Modulate = data.IconModulate;

            if (data.Contents == string.Empty)
                _contentsLabel.Visible = false;
        }

        private void PlayDisplayAnimation()
        {
            Visible = true;
            Modulate = new Color(1, 1, 1, 0.0f);

            var colorTarget = new Color(1, 1, 1, 1f);
            var sizeTarget = new Vector2(400f, 100f);

            var tween = CreateTween().SetParallel();
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "rect_min_size", sizeTarget, _tweenDuration * 2f).SetTrans(Tween.TransitionType.Linear);
            tween.TweenProperty(this, "modulate", colorTarget, _tweenDuration).SetTrans(Tween.TransitionType.Linear);
        }
        private void PlayHideAnimation()
        {
            var colorTarget = new Color(1, 1, 1, 0.0f);
            var sizeTarget = new Vector2(400f, 0f);

            var tween = CreateTween().SetParallel();
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "rect_min_size", sizeTarget, _tweenDuration * 4f).SetTrans(Tween.TransitionType.Back);
            tween.TweenProperty(this, "modulate", colorTarget, _tweenDuration * 2f).SetTrans(Tween.TransitionType.Linear);
            tween.Chain().TweenCallback(this, "queue_free").SetDelay(_tweenDuration);
        }

        private void GetNodes()
        {
            _titleLabel = GetNode<Label>("%Title");
            _contentsLabel = GetNode<Label>("%Contents");
            _icon = GetNode<TextureRect>("%Icon");
        }

        private float _tweenDuration = 0.1f;
        private Label _titleLabel, _contentsLabel;
        private TextureRect _icon;
    }
}
