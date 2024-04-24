using Godot;
using Jump.Utils.SaveData;

namespace Jump.UI.Settings;

public abstract class MaxFpsProperty : SliderProperty
{
    protected override void OnUpdateElements(ConfigSaveData data)
    {
        if (OS.VsyncEnabled)
        {
            MakeInactive();
        }
        else
        {
            MakeActive();
        }

        slider.Value = data.Settings.MaxFps;
    }

    protected override void OnValueChanged(float value)
    {
        data.Settings.MaxFps = (int)value;
        Engine.TargetFps = data.Settings.MaxFps;
    }

    private void MakeInactive()
    {
        slider.Editable = false;
        spinBox.Editable = false;
        Modulate = new Color(1f, 1f, 1f, 0.5f);
    }

    private void MakeActive()
    {
        slider.Editable = true;
        spinBox.Editable = true;
        Modulate = new Color(1f, 1f, 1f, 1f);
    }
}