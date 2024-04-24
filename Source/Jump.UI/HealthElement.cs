using Godot;
using System;

namespace Jump.UI
{
    public class HealthElement : MarginContainer
    {
        [Export] private NodePath _heartFullPath;
        [Export] private NodePath _heartEmptyPath;

        private TextureRect _heartFullRect;
        private TextureRect _heartEmptyRect;

        private bool _deducted;

        public override void _Ready()
        {
            _heartFullRect = GetNode<TextureRect>(_heartFullPath);
            _heartEmptyRect = GetNode<TextureRect>(_heartEmptyPath);
        }

        public void MakeFull()
        {
            _heartEmptyRect.Visible = false;
            _heartFullRect.Visible = true;
        }

        public void MakeEmpty()
        {
            _heartEmptyRect.Visible = true;
            _heartFullRect.Visible = false;
        }

        public void Deduct()
        {
            // MakeEmpty();
            _heartEmptyRect.Visible = true;
            var tween = CreateTween();
            tween.SetTrans(Tween.TransitionType.Back);
            tween.SetEase(Tween.EaseType.Out);
            tween.Parallel().TweenProperty(_heartFullRect, "rect_position", new Vector2(0f, -32f), 0.35f);
            tween.Parallel().TweenProperty(_heartFullRect, "self_modulate", new Color(1, 1, 1, 0), 0.5f);
        }

        public void Refill()
        {
            _heartEmptyRect.Visible = false;
            var tween = CreateTween();
            tween.SetTrans(Tween.TransitionType.Back);
            tween.SetEase(Tween.EaseType.Out);
            tween.Parallel().TweenProperty(_heartFullRect, "rect_position", new Vector2(0f, 0f), 0.35f).FromCurrent();
            tween.Parallel().TweenProperty(_heartFullRect, "self_modulate", new Color(1, 1, 1, 1), 0.5f);
        }
    }
}