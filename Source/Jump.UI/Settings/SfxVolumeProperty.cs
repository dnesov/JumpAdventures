using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public abstract class SfxVolumeProperty : VolumeProperty
    {
        protected override void OnUpdateElements(ConfigSaveData data)
        {
            slider.Value = data.Settings.AudioSettings.SfxVolume;
        }
        protected override void OnValueChanged(float value)
        {
            data.Settings.AudioSettings.SfxVolume = value;
            fmodRuntime.GetBus("bus:/SFX").Volume = value;
        }
    }
}
