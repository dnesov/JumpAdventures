using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public class TimerProperty : OptionSettingsProperty
    {
        public override void _Ready()
        {
            base._Ready();
            Display();
        }
        protected override void OnDisplay()
        {
            optionButton.Select(data.TimerEnabled ? 1 : 0);
        }

        protected override void OnHide()
        {

        }

        protected override void OnUpdateElements(ConfigSaveData data)
        {

        }
        protected override void ValueChanged(int value)
        {
            data.TimerEnabled = value == 1;
        }
    }
}