using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public class FlashingProperty : OptionSettingsProperty
    {
        protected override void OnDisplay()
        {
            optionButton.Select(data.Settings.AccessibilitySettings.FlashingEnabled ? 1 : 0);
        }

        protected override void OnHide()
        {

        }

        protected override void OnUpdateElements(ConfigSaveData data)
        {
            optionButton.Select(data.Settings.AccessibilitySettings.FlashingEnabled ? 1 : 0);
        }

        protected override void ValueChanged(int value)
        {
            data.Settings.AccessibilitySettings.FlashingEnabled = value == 1;
        }
    }
}