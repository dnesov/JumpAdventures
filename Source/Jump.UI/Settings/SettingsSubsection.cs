using Godot;
using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public abstract class SettingsSubsection : UIElement<ConfigSaveData>
    {
        protected override void OnUpdateElements(ConfigSaveData data)
        {
            base.OnUpdateElements();
            var propertyList = GetNode<VBoxContainer>("PropertyList").GetChildren();
            foreach (SettingsProperty property in propertyList)
            {
                if (property.IsDisabled()) continue;

                if (property.Subsection == null)
                {
                    property.Subsection = this;
                }

                property.UpdateElements(data);
            }
        }
    }
}