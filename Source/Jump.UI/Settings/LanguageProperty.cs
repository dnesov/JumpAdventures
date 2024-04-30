using System.Collections.Generic;
using System.Linq;
using Godot;
using Jump.Utils.SaveData;

namespace Jump.UI.Settings
{
    public abstract class LanguageProperty : OptionSettingsProperty
    {
        public override void _Ready()
        {
            base._Ready();
            Display();
        }
        protected override void OnDisplay()
        {
            foreach (var language in Constants.SupportedLanguages.Values)
            {
                optionButton.AddItem(language);
            }

            var localeIndex = Constants.SupportedLanguages.Keys.ToList().IndexOf(TranslationServer.GetLocale());
            optionButton.Select(localeIndex);
        }
        protected override void OnUpdateElements(ConfigSaveData data)
        {

        }
        protected override void ValueChanged(int value)
        {
            var locale = Constants.SupportedLanguages.ElementAt(value).Key;
            data.Settings.Locale = locale;
            TranslationServer.SetLocale(locale);
        }
    }
}