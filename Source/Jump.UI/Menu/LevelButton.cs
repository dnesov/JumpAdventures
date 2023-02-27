using Godot;
using Jump.Levels;
using System;

namespace Jump.UI.Menu
{
    public class LevelButton : UIElement<LevelButtonData>
    {
        public short LevelId { get; set; }
        public Action<short> OnPressed;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            GetNode<TextureButton>("%Button").Connect("pressed", this, nameof(ButtonPressed));
        }
        private void ButtonPressed() => OnPressed?.Invoke(LevelId);

        protected override void OnDisplay()
        {
            throw new NotImplementedException();
        }

        protected override void OnHide()
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateElements(LevelButtonData data)
        {
            var levelName = data.IsUser ? data.Level.Name : String.Format(Tr(data.Level.Name), data.Order);
            GetNode<Label>("%Name").Text = levelName;
            GetNode<TextureRect>("%CompleteIcon").Visible = data.Completed;
        }

        private Level _level;
    }

    public class LevelButtonData
    {
        public bool IsUser;
        public int Order;
        public Level Level;
        public bool Completed;
    }
}