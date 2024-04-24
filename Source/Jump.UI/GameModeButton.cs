using Godot;
using System;

namespace Jump.UI
{
    public class GameModeButton : JAButton
    {
        protected override void OnFocusEntered()
        {
            PlayFocusAnimation();
        }

        protected override void OnFocusExited()
        {
            PlayFocusExitAnimation();
        }

        protected override void OnPressed()
        {
            // throw new NotImplementedException();
        }

        private void PlayFocusAnimation()
        {
            var panel = GetChild<PanelContainer>(0);
            var styleBox = panel.GetStylebox("panel");
            var tween = CreateTween();
            tween.SetParallel();
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(styleBox, "border_color:a", 1f, tweenTime);
            tween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 1f), tweenTime);
        }

        private void PlayFocusExitAnimation()
        {
            var panel = GetChild<PanelContainer>(0);
            var styleBox = panel.GetStylebox("panel");
            var tween = CreateTween();
            tween.SetParallel();
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(styleBox, "border_color:a", 0f, tweenTime);
            tween.TweenProperty(this, "modulate", new Color(0.6f, 0.6f, 0.6f, 1f), tweenTime);
        }
    }
}