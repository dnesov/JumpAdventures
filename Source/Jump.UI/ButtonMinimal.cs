using Godot;
using GodotFmod;
using System;

namespace Jump.UI
{
    [Tool]
    public class ButtonMinimal : JAButton
    {
        [Export]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                UpdateLabel(_text);
            }
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            _borders = GetNode<PanelContainer>("%Borders");

            RectPivotOffset = RectSize / 2f;

            if (!Disabled) return;
            Modulate = new Color(1.0f, 1.0f, 1.0f, 0.1f);
        }

        private void UpdateLabel(string text)
        {
            var label = GetNode<Label>("%Label");
            label.Text = text;
        }

        private void PlayPressedSound()
        {
            fmodRuntime.PlayOneShot(buttonPressEvent);
        }

        protected override void OnPressed()
        {
            var tween = CreateTween().SetParallel();
            tween.SetTrans(Tween.TransitionType.Back);
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "rect_scale", new Vector2(0.8f, 0.8f), tweenTime / 2f);
            tween.Chain().TweenProperty(this, "rect_scale", new Vector2(1f, 1f), tweenTime);

            PlayPressedSound();
        }

        protected override void OnFocusEntered()
        {
            var tween = _borders.CreateTween().SetParallel();
            tween.SetTrans(Tween.TransitionType.Linear);
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 1f), tweenTime);

            fmodRuntime.PlayOneShot(buttonHoverEvent);
        }

        protected override void OnFocusExited()
        {
            var tween = _borders?.CreateTween()?.SetParallel();
            tween?.SetTrans(Tween.TransitionType.Linear);
            tween?.SetEase(Tween.EaseType.Out);
            tween?.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 0.25f), tweenTime);
        }

        private PanelContainer _borders;
        private string _text;
    }
}