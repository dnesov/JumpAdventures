using Godot;
using System;
using Jump.Utils;

namespace Jump.UI.Menu
{
    public class WorldButton : UIElement<WorldButtonData>
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

        protected override void OnUpdateElements(WorldButtonData data)
        {
            _world = data.World;

            var orderFormatted = String.Format(Tr("UI_WORLD_ORDER"), Order);
            GetNode<Label>("%Order").Text = orderFormatted;
            GetNode<Label>("%Name").Text = Tr(_world.Name);
            GetNode<Control>("%UnavailableOverlay").Visible = !_world.Playable;

            if (data.WorldSaveData == null) return;
            var completedLevels = data.WorldSaveData.GetCompletedLevelsAmount();
            float completedPercentage = (float)Math.Round((float)completedLevels / (float)_world.Levels.Length * 100f);
            GetNode<Label>("%Progress").Text = $"{completedLevels}/{_world.Levels.Length} ({completedPercentage}%)";
            GetNode<Label>("%FragmentCount").Text = $"{data.WorldSaveData.FragmentsCollected}/6";

            if (_world.Image == null) return;
            GetNode<TextureRect>("%WorldImage").Texture = _world.Image;
        }

        protected override void OnDisplay()
        {

        }

        protected override void OnHide()
        {

        }
        private Jump.Levels.World _world;
    }

    public class WorldButtonData
    {
        public Levels.World World;
        public WorldSaveData WorldSaveData;
    }
}
