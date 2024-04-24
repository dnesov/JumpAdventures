using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public class ScreenShakeProperty : SliderProperty
    {
        protected override void OnDisplay()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnUpdateElements(ConfigSaveData data)
        {
            slider.Value = data.Settings.AccessibilitySettings.ScreenShake;
        }

        protected override void OnValueChanged(float value)
        {
            base.OnValueChanged(value);
            var settings = data.Settings.AccessibilitySettings;
            settings.ScreenShake = value;
        }
    }
}