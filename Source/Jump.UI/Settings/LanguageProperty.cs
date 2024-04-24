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
            foreach (var language in _languages.Values)
            {
                optionButton.AddItem(language);
            }

            var localeIndex = _languages.Keys.ToList().IndexOf(TranslationServer.GetLocale());
            optionButton.Select(localeIndex);
        }
        protected override void OnUpdateElements(ConfigSaveData data)
        {

        }
        protected override void ValueChanged(int value)
        {
            var locale = _languages.ElementAt(value).Key;
            data.Settings.Locale = locale;
            TranslationServer.SetLocale(locale);
        }

        // TODO: don't do this here.
        private readonly Dictionary<string, string> _languages = new Dictionary<string, string>()
        {
            {"en", "English"},
            {"pl", "Polski"},
            {"uk", "Українська"},
            {"ru", "Русский"},
        };
    }
}