using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public abstract class MusicVolumeProperty : VolumeProperty
    {
        protected override void OnUpdateElements(ConfigSaveData data)
        {
            slider.Value = data.Settings.AudioSettings.MusicVolume;
        }
        protected override void OnValueChanged(float value)
        {
            data.Settings.AudioSettings.MusicVolume = value;
            fmodRuntime.GetBus("bus:/Music").Volume = value;
        }
    }
}
