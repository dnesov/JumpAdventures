using Godot;
using System;

namespace Jump.UI
{
    public class TransitionUI : UIElement
    {
        public Action OnTransitionEnd;
        public Action OnCanRespawn;

        public override void _Ready()
        {
            base._Ready();
            _transitionRect = GetNode<ColorRect>("TransitionRect");
        }
        protected override void OnDisplay()
        {
            SetUniforms();
            PlayDisplayAnimation();
        }

        protected override void OnHide()
        {
            SetUniforms();
            PlayHideAnimation();
        }

        private void SetUniforms()
        {
            var screenSize = GetViewport().Size;

            var material = _transitionRect.Material as ShaderMaterial;
            material.SetShaderParam("screen_width", screenSize.x);
            material.SetShaderParam("screen_height", screenSize.y);
        }

        private void PlayDisplayAnimation()
        {
            if (_transitionInProgress) return;
            _transitionInProgress = true;

            var tween = CreateTween();
            tween.TweenProperty(_transitionRect.Material, "shader_param/circle_size", 0.0f, _transitionSpeed);
            tween.TweenInterval(_transitionSpeed);
            tween.TweenCallback(this, nameof(CanRespawn));
            tween.TweenProperty(_transitionRect.Material, "shader_param/circle_size", 1.05f, _transitionSpeed);
            tween.TweenCallback(this, nameof(TransitionEnd));

        }

        private void CanRespawn() => OnCanRespawn?.Invoke();

        private void TransitionEnd()
        {
            _transitionInProgress = false;
            OnTransitionEnd?.Invoke();
        }

        private void PlayHideAnimation()
        {
            var tween = CreateTween();
            tween.TweenProperty(_transitionRect.Material, "shader_param/circle_size", 1.05f, _transitionSpeed);
        }

        private bool _transitionInProgress;
        private ColorRect _transitionRect;
        [Export] private float _transitionSpeed = 0.2f;
    }
}