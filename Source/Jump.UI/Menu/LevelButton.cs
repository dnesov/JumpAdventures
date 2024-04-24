using Godot;
using Jump.Levels;
using System;

namespace Jump.UI.Menu
{
    public class LevelButton : JAButton<LevelButtonData>
    {
        public short LevelId { get; set; }
        public new Action<short> OnPressedAction;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
        }

        protected override void OnUpdateElements(LevelButtonData data)
        {
            var levelName = data.IsUser ? data.Level.Name : string.Format(Tr(data.Level.Name), data.Order);
            GetNode<Label>("%Name").Text = levelName;
            GetNode<TextureRect>("%CompleteIcon").Visible = data.Completed;


            if (data.Level.MaxEssence > 0)
            {
                GetNode<Container>("%EssenceContainer").Visible = true;
                GetNode<Label>("%EssenceAmount").Text = data.Level.MaxEssence.ToString();
            }
        }

        protected override void OnPressed()
        {
            OnPressedAction?.Invoke(LevelId);
        }

        protected override void OnFocusEntered()
        {
            PlayFocusAnimation();
        }

        protected override void OnFocusExited()
        {
            PlayFocusExitAnimation();
        }

        private void PlayFocusAnimation()
        {
            var outline = GetNode<PanelContainer>("%Outline");
            var tween = CreateTween();
            tween.SetParallel();
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(outline, "modulate", new Color(1f, 1f, 1f, 1f), tweenTime);
        }

        private void PlayFocusExitAnimation()
        {
            var outline = GetNode<PanelContainer>("%Outline");
            var tween = CreateTween();
            tween.SetParallel();
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(outline, "modulate", new Color(1f, 1f, 1f, 0.0f), tweenTime);
        }
    }

    public class LevelButtonData
    {
        public bool IsUser;
        public int Order;
        public Level Level;
        public bool Completed;
    }
}