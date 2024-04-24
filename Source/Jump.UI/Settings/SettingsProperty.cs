using Godot;
using Jump.Extensions;
using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public abstract class SettingsProperty : UIElement<ConfigSaveData>
    {
        public SettingsSubsection Subsection { get; set; }
        public override void _Ready()
        {
            base._Ready();

            game = this.GetSingleton<Game>();

            nameLabel = GetNode<Label>("Name");

            data = game.DataHandler.Data;
        }

        public abstract bool IsDisabled();

        protected ConfigSaveData data;
        protected Label nameLabel;
        protected Game game;
    }
}