using Godot;
using Jump.Customize;
using Jump.Extensions;
using Jump.Unlocks;
using Jump.Utils;
using System;

namespace Jump.UI.Menu
{
    public class CustomizeSection : MenuSection
    {
        public string CurrentSkin => _customizationHandler.Preferences.SkinId;
        public string CurrentColor => _customizationHandler.Preferences.ColorId;
        public string CurrentTrail => _customizationHandler.Preferences.TrailId;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            _customizationHandler = this.GetSingleton<CustomizationHandler>();
            _progressHandler = this.GetSingleton<ProgressHandler>();
            _database = this.GetSingleton<UnlocksDatabase>();

            _unlockConfirm = GetNode<UnlockConfirm>("%UnlockConfirm");

            SubscribeEvents();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        public override void Focus()
        {
            base.Focus();
            var section = GetNode<GridContainer>("%SkinSubsection");
            var button = section.GetChild<JAButton>(0);
            button.GrabFocus();
        }

        public void SetHintLabel(string message)
        {
            var hintLabel = GetNode<RichTextLabel>("%Hint");
            hintLabel.BbcodeText = message;
        }

        public void TryUnlock(UnlockableBase unlockable)
        {
            var needsEssence = unlockable.HasCondition<EssenceUnlockCondition>();

            if (needsEssence)
            {
                _unlockConfirm.UpdateElements(unlockable);
                _unlockConfirm.Display();
            }
            else
            {
                _lastPressedButton?.PlayUnlockAnimation();
                _lastPressedButton?.PlayUnlockSound();
                _lastPressedButton.MakeUnlocked();

                _database.TryUnlock(unlockable.EntryId);
            }
        }

        public void TryDeductEssence(UnlockableBase unlockable)
        {
            var canUnlock = _database.CanUnlock(unlockable.EntryId);
            if (!canUnlock) return;

            _database.TryUnlock(unlockable.EntryId);

            var condition = unlockable.GetCondition<EssenceUnlockCondition>();
            _progressHandler.Essence -= condition.EssenceRequired;

            _lastPressedButton.PlayUnlockAnimation();
            _lastPressedButton.PlayUnlockSound();
            _lastPressedButton.MakeUnlocked();
            _lastPressedButton.GrabFocus();

            UpdateElements();
        }

        protected override void OnDisplay()
        {
            base.OnDisplay();
            PlayDisplayAnimation();
            UpdateElements();
            Focus();
        }

        protected override void OnUpdateElements()
        {
            UpdateCollectiblesLabels();
            UpdateCustomizeButtons();
            UpdateCharacterPreview();
        }

        protected override void OnHide()
        {
            base.OnHide();
            PlayHideAnimation();
        }

        protected override void OnGoBack()
        {
            Hide();
            menu.DisplayMainSection();
        }

        private void UpdateCollectiblesLabels()
        {
            GetNode<Label>("%EssenceLabel").Text = _progressHandler.Essence.ToString();
            GetNode<Label>("%FragmentLabel").Text = _progressHandler.GlobalFragments.ToString();
            GetNode<Label>("%ExperienceLabel").Text = _progressHandler.Experience.ToString();
        }

        private void UpdateCharacterPreview()
        {
            var preview = GetNode<TextureRect>("%SkinPreview");
            var prefs = _customizationHandler.Preferences;
            preview.Modulate = _customizationHandler.GetColorById(prefs.ColorId);
            preview.Texture = _customizationHandler.GetSkinById(prefs.SkinId);
        }

        private void UpdateCustomizeButtons()
        {
            UpdateSkinSubsection();
            UpdateColorSubsection();
            UpdateTrailSubsection();
        }

        private void UpdateSkinSubsection()
        {
            var skinSubsection = GetNode<GridContainer>("%SkinSubsection");
            foreach (CustomizeButton child in skinSubsection.GetChildren())
            {
                if (child.SkinId == CurrentSkin)
                {
                    child.MakeSelected();
                }
                else
                {
                    child.MakeDeselected();
                }

                child.MakeUnlocked();

                var unlockId = child.UnlockId;
                if (!child.Unlockable) continue;

                var canUnlock = _database.CanUnlock(unlockId);
                var unlocked = _database.IsUnlocked(unlockId);

                if (unlocked)
                {
                    child.MakeUnlocked();
                }
                else
                {
                    child.MakeLocked();
                }

                if (canUnlock && !unlocked)
                {
                    child.MakeReadyToUnlock();
                }
            }
        }

        private void UpdateColorSubsection()
        {
            var colorSubsection = GetNode<GridContainer>("%ColorSubsection");
            foreach (CustomizeButton child in colorSubsection.GetChildren())
            {
                if (child.ColorId == CurrentColor)
                {
                    child.MakeSelected();
                }
                else
                {
                    child.MakeDeselected();
                }

                child.MakeUnlocked();

                var unlockId = child.UnlockId;
                if (!child.Unlockable) continue;

                var canUnlock = _database.CanUnlock(unlockId);
                var unlocked = _database.IsUnlocked(unlockId);

                if (unlocked)
                {
                    child.MakeUnlocked();
                }
                else
                {
                    child.MakeLocked();
                }

                if (canUnlock && !unlocked)
                {
                    child.MakeReadyToUnlock();
                }
            }
        }

        private void UpdateTrailSubsection()
        {
            var trailSubsection = GetNode<GridContainer>("%TrailSubsection");
            foreach (CustomizeButton child in trailSubsection.GetChildren())
            {
                if (child.TrailId == CurrentTrail)
                {
                    child.MakeSelected();
                }
                else
                {
                    child.MakeDeselected();
                }

                child.MakeUnlocked();

                var unlockId = child.UnlockId;
                if (!child.Unlockable) continue;

                var canUnlock = _database.CanUnlock(unlockId);
                var unlocked = _database.IsUnlocked(unlockId);

                if (unlocked)
                {
                    child.MakeUnlocked();
                }
                else
                {
                    child.MakeLocked();
                }

                if (canUnlock && !unlocked)
                {
                    child.MakeReadyToUnlock();
                }
            }
        }

        private void OnUnlockConfirmExited()
        {
            if (_lastPressedButton == null) return;
            _lastPressedButton.GrabFocus();
        }

        private void SubscribeEvents()
        {
            _unlockConfirm.OnConfirmed += TryDeductEssence;
            _unlockConfirm.OnExited += OnUnlockConfirmExited;

            SubscribeButtonEvents();
        }

        private void UnsubscribeEvents()
        {
            _unlockConfirm.OnConfirmed -= TryDeductEssence;
            _unlockConfirm.OnExited -= OnUnlockConfirmExited;

            UnsubscribeButtonEvents();
        }

        private void SubscribeButtonEvents()
        {
            var skinSubsection = GetNode<GridContainer>("%SkinSubsection");
            var colorSubsection = GetNode<GridContainer>("%ColorSubsection");
            var trailSubsection = GetNode<GridContainer>("%TrailSubsection");

            foreach (CustomizeButton child in skinSubsection.GetChildren())
            {
                child.OnPressedAction += ButtonPressed;
                child.OnFocusEnteredAction += ButtonFocused;
            }

            foreach (CustomizeButton child in colorSubsection.GetChildren())
            {
                child.OnPressedAction += ButtonPressed;
                child.OnFocusEnteredAction += ButtonFocused;
            }

            foreach (CustomizeButton child in trailSubsection.GetChildren())
            {
                child.OnPressedAction += ButtonPressed;
                child.OnFocusEnteredAction += ButtonFocused;
            }
        }

        private void UnsubscribeButtonEvents()
        {
            var skinSubsection = GetNode<GridContainer>("%SkinSubsection");
            var colorSubsection = GetNode<GridContainer>("%ColorSubsection");
            var trailSubsection = GetNode<GridContainer>("%TrailSubsection");

            foreach (CustomizeButton child in skinSubsection.GetChildren())
            {
                child.OnPressedAction -= ButtonPressed;
                child.OnFocusEnteredAction -= ButtonFocused;
            }

            foreach (CustomizeButton child in colorSubsection.GetChildren())
            {
                child.OnPressedAction -= ButtonPressed;
                child.OnFocusEnteredAction -= ButtonFocused;
            }

            foreach (CustomizeButton child in trailSubsection.GetChildren())
            {
                child.OnPressedAction -= ButtonPressed;
                child.OnFocusEnteredAction -= ButtonFocused;
            }
        }

        private void ButtonPressed(CustomizeButton button)
        {
            _lastPressedButton = button;

            if (button.State == CustomizeButtonState.Unlocked)
            {
                var skinId = button.SkinId == string.Empty ? CurrentSkin : button.SkinId;
                var colorId = button.ColorId == string.Empty ? CurrentColor : button.ColorId;
                var trailId = button.TrailId == string.Empty ? CurrentTrail : button.TrailId;

                var prefs = new CustomizationPreferences()
                {
                    SkinId = skinId,
                    ColorId = colorId,
                    TrailId = trailId
                };

                _customizationHandler.UpdatePreferences(prefs);
            }

            UpdateElements();

            if (!button.Unlockable || button.State != CustomizeButtonState.ReadyToUnlock) return;
            var unlockable = _database.GetUnlockable(button.UnlockId);
            TryUnlock(unlockable);
        }

        private void ButtonFocused(CustomizeButton button)
        {
            var unlocked = _database.IsUnlocked(button.UnlockId);

            if (!button.Unlockable || unlocked)
            {
                SetHintLabel(string.Empty);
                return;
            }

            var unlockable = _database.GetUnlockable(button.UnlockId);

            string hint = unlockable.FormattedDescription;
            SetHintLabel(hint);
        }

        private CustomizeButton _lastPressedButton;
        private CustomizationHandler _customizationHandler;
        private ProgressHandler _progressHandler;
        private UnlocksDatabase _database;
        private UnlockConfirm _unlockConfirm;
        private Logger _logger = new Logger(nameof(CustomizeSection));
    }
}