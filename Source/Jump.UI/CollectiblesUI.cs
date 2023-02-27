using Godot;
using System;

namespace Jump.UI
{
    public class CollectiblesUI : UIElement
    {
        public override void _Ready()
        {
            base._Ready();
            _game = GetTree().Root.GetNode<Game>("Game");
            _essenceContainer = GetNode<HBoxContainer>("%EssenceContainer");
            _game.OnEssenceChanged += OnUpdateElements;

            _game.OnWin += OnHide;
        }
        public override void _ExitTree()
        {
            base._ExitTree();
            _game.OnEssenceChanged -= OnUpdateElements;
            _game.OnWin -= OnHide;
        }
        protected override void OnDisplay()
        {
            throw new NotImplementedException();
        }

        protected override void OnHide()
        {
            Visible = false;
        }

        protected override void OnUpdateElements()
        {
            var tween = _essenceContainer.CreateTween();
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(_essenceContainer, "rect_scale", new Vector2(1.1f, 1.1f), 0.1f);
            tween.Chain().TweenProperty(_essenceContainer, "rect_scale", Vector2.One, 0.15f);
            GetNode<Label>("%EssenceLabel").Text = _game.SessionEssence.ToString();
        }

        private HBoxContainer _essenceContainer;
        private Game _game;
    }
}