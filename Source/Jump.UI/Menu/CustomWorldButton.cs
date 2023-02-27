using Godot;
using System;

namespace Jump.UI.Menu
{
    public class CustomWorldButton : UIElement<Levels.World>
    {
        public int Order { get; set; }
        public Action<int> OnPressed;
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            ConnectSignals();
        }

        private void ConnectSignals()
        {
            GetNode<TextureButton>("%Button").Connect("pressed", this, nameof(ButtonPressed));
        }

        private void ButtonPressed()
        {
            if (!_world.Playable) return;
            OnPressed?.Invoke(Order - 1);
        }

        protected override void OnUpdateElements(Levels.World data)
        {
            _world = data;

            GetNode<Label>("%Author").Text = data.Author;
            GetNode<Label>("%Name").Text = data.Name;

            if (data.Image == null) return;
            GetNode<TextureRect>("%WorldImage").Texture = data.Image;
        }

        protected override void OnDisplay()
        {

        }

        protected override void OnHide()
        {

        }

        private Jump.Levels.World _world;
    }
}
