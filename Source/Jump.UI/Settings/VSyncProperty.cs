using Godot;
using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public abstract class VSyncProperty : OptionSettingsProperty
    {
        protected override void OnUpdateElements(ConfigSaveData data)
        {
            optionButton.Select(data.Settings.VSync ? 1 : 0);
        }
        protected override void ValueChanged(int value)
        {
            data.Settings.VSync = value == 1 ? true : false;
            OS.VsyncEnabled = value == 1 ? true : false;
            Subsection.UpdateElements(data);
        }
    }
}