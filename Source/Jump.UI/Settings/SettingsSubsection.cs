using System;
using Godot;

namespace Jump.UI.Settings
{
    public abstract class SettingsSubsection : UIElement<GlobalData>
    {
        protected override void OnUpdateElements(GlobalData data)
        {
            base.OnUpdateElements();
            var propertyList = GetNode<VBoxContainer>("PropertyList").GetChildren();
            foreach (SettingsProperty property in propertyList)
            {
                if (property.IsDisabled()) continue;
                property.UpdateElements(data);
            }
        }
    }
}