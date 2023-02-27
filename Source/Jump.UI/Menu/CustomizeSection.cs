using Godot;
using Jump.Customize;
using Jump.Unlocks;
using System;

namespace Jump.UI.Menu
{
    public class CustomizeSection : MenuSection
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            GetNode<ButtonMinimal>("%Back").OnPressedAction += GoBack;
            _customizationHandler = GetTree().Root.GetNode<CustomizationHandler>("CustomizationHandler");
            _game = GetTree().Root.GetNode<Game>("Game");
        }

        protected override void OnDisplay()
        {
            PlayDisplayAnimation();
            UpdateEssenceLabel();
            UpdatePreview();
            PopulateColorsSection();
            PopulateSkinsSection();
        }

        private void UpdateEssenceLabel()
        {
            GetNode<Label>("%EssenceLabel").Text = _game.DataHandler.Data.SaveData.Essence.ToString();
        }
        protected override void OnHide() => PlayHideAnimation();

        private void PopulateColorsSection()
        {
            var section = GetNode<GridContainer>("%ColorSubsection");
            if (section.GetChildCount() != 0) return;

            foreach (var color in _customizationHandler.Colors)
            {
                var button = CreateCustomizeButton();
                section.AddChild(button);

                var unlockId = _customizationHandler.GetColorUnlockId(color.Key);

                button.UpdateElements(new CustomizeButtonData()
                {
                    SkinId = "",
                    ColorId = color.Key,
                    PreviewModulate = color.Value,
                    UnlockId = unlockId,
                });
            }
        }

        private void PopulateSkinsSection()
        {
            var section = GetNode<GridContainer>("%SkinSubsection");
            if (section.GetChildCount() != 0) return;

            foreach (var skin in _customizationHandler.Skins)
            {
                var button = CreateCustomizeButton();
                section.AddChild(button);

                var unlockId = _customizationHandler.GetSkinUnlockId(skin.Key);

                button.UpdateElements(new CustomizeButtonData()
                {
                    SkinId = skin.Key,
                    ColorId = "",
                    PreviewTexture = skin.Value,
                    UnlockId = unlockId,
                });
            }
        }

        private void UpdatePreview()
        {
            var preview = GetNode<TextureRect>("%SkinPreview");
            var prefs = _customizationHandler.Preferences;
            preview.Modulate = _customizationHandler.GetColorById(prefs.ColorId);
            preview.Texture = _customizationHandler.GetSkinById(prefs.SkinId);
        }

        private CustomizeButton CreateCustomizeButton()
        {
            var button = _customizeButtonScene.Instance<CustomizeButton>();
            button.OnPressedAction += CustomizeButtonPressed;
            button.OnFocusedAction += CustomizeButtonFocused;
            return button;
        }

        private void CustomizeButtonPressed(string skinId, string colorId)
        {
            var previousSkinId = _customizationHandler.Preferences.SkinId;
            var previousColorId = _customizationHandler.Preferences.ColorId;

            _customizationHandler.Preferences.SkinId = skinId == string.Empty ? previousSkinId : skinId;
            _customizationHandler.Preferences.ColorId = colorId == string.Empty ? previousColorId : colorId;
            UpdatePreview();

            // if (unlockId != string.Empty) TryDeductEssence(unlockId);
        }

        private void TryDeductEssence(string unlockId)
        {
            var database = GetTree().Root.GetNode<UnlocksDatabase>("UnlocksDatabase");
            var unlock = database.GetUnlockable(unlockId);
            var essenceCondition = unlock.GetCondition<EssenceUnlockCondition>();

            if (!database.IsUnlocked(unlockId))
                _game.DataHandler.Data.SaveData.Essence -= essenceCondition.EssenceRequired;

            UpdateEssenceLabel();
        }

        private void CustomizeButtonFocused(bool unlocked, string focusMessage)
        {
            // if (unlocked) return;
            var hintLabel = GetNode<RichTextLabel>("%Hint");
            hintLabel.BbcodeText = focusMessage;
        }

        protected override void GoBack()
        {
            Hide();
            menu.DisplayExtrasSection();
        }

        [Export] private PackedScene _customizeButtonScene;
        private CustomizationHandler _customizationHandler;
        private Game _game;
    }
}
