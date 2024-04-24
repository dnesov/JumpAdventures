using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public abstract class MasterVolumeProperty : VolumeProperty
    {
        protected override void OnUpdateElements(ConfigSaveData data)
        {
            slider.Value = data.Settings.AudioSettings.MasterVolume;
        }
        protected override void OnValueChanged(float value)
        {
            data.Settings.AudioSettings.MasterVolume = value;
            fmodRuntime.GetBus("bus:/").Volume = value;
        }
    }
}
