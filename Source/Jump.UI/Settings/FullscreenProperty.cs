using Godot;
using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public abstract class FullscreenProperty : OptionSettingsProperty
    {
        protected override void OnUpdateElements(ConfigSaveData data)
        {
            optionButton.Select(data.Settings.Fullscreen ? 1 : 0);
        }
        protected override void ValueChanged(int value)
        {
            data.Settings.ToggleFullscreen(value == 1 ? true : false);
            OS.WindowFullscreen = value == 1;
        }
    }
}