using Godot;
using GodotFmod;
using Jump.UI.Settings;
using System;

namespace Jump.UI
{
    public class PauseMenu : UIElement<PauseMenuData>
    {
        public override void _Ready()
        {
            _game = GetTree().Root.GetNode<Game>("Game");
            _fmodRuntime = GetTree().Root.GetNode<FmodRuntime>("FmodRuntime");

            _game.OnPaused += OnPaused;
            _game.OnResumed += OnResumed;

            Hide();
        }

        public override void _ExitTree()
        {
            _game.OnPaused -= OnPaused;
            _game.OnResumed -= OnResumed;
        }
        private void OnPaused() => Display();
        private void OnResumed() => Hide();
        private void Resume() => _game.Resume();

        private void PlayDisplayAnimation()
        {
            var overlay = GetNode<ColorRect>("%Overlay");
            var mainNav = GetNode<VBoxContainer>("%MainNav");

            var tween = CreateTween().SetParallel();
            tween.SetTrans(Tween.TransitionType.Cubic);
            tween.SetEase(Tween.EaseType.Out);
            tween.Chain().TweenProperty(this, "visible", true, 0f);
            tween.TweenProperty(overlay, "modulate", new Color(1f, 1f, 1f, 1f), 0.3f);
            tween.TweenProperty(mainNav, "rect_position", new Vector2(80, 0), 0.4f);
            tween.TweenProperty(mainNav, "modulate", new Color(1f, 1f, 1f, 1f), 0.4f);
            Visible = true;
        }

        private void PlayHideAnimation()
        {
            var overlay = GetNode<ColorRect>("%Overlay");
            var mainNav = GetNode<VBoxContainer>("%MainNav");

            var tween = CreateTween().SetParallel();
            tween.SetTrans(Tween.TransitionType.Cubic);
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(overlay, "modulate", new Color(1f, 1f, 1f, 0f), 0.3f);
            tween.TweenProperty(mainNav, "rect_position", new Vector2(-200, 0), 0.5f);
            tween.TweenProperty(mainNav, "modulate", new Color(1f, 1f, 1f, 0f), 0.4f);
        }

        private void PlayDisplaySound()
        {
            _fmodRuntime.PlayOneShot("event:/UI/SectionClick");
            _fmodRuntime.GetBus("bus:/SFX").Paused = true;
            _fmodRuntime.SetParameter("Filter", 1f);
        }

        private void MainMenu()
        {
            Resume();
            _game.ReturnToMenu(true);
            _logger.Info("Switching to main menu.");
        }

        private void ShowSettings()
        {
            var settingsSection = GetNode<PauseSettingsSection>("%PauseSettingsSection");
            settingsSection.Display();
        }
        private void HideSettings()
        {
            var settingsSection = GetNode<PauseSettingsSection>("%PauseSettingsSection");
            settingsSection.Hide();
        }

        protected override void OnDisplay()
        {
            PlayDisplayAnimation();
            GetNode<TextureButton>("%Continue").GrabFocus();
            PlayDisplaySound();

            // TODO: do not use Game to retrieve data; pass it separately instead.
            UpdateElements(new PauseMenuData()
            {
                Attempts = _game.Attempts,
                CurrentLevelName = _game.CurrentLevel.Name,
                CurrentWorldName = _game.CurrentWorld.Name,
                FragmentsCollected = _game.CurrentWorldSaveData.FragmentsCollected
            });
        }

        protected override void OnHide()
        {
            PlayHideAnimation();
            foreach (TextureButton button in GetNode<VBoxContainer>("%Buttons").GetChildren())
            {
                button.ReleaseFocus();
            }
            _fmodRuntime.GetBus("bus:/SFX").Paused = false;
            _fmodRuntime.SetParameter("Filter", 0f);

            HideSettings();
        }

        protected override void OnUpdateElements(PauseMenuData data)
        {
            var attemptsFormatted = String.Format(Tr("UI_PAUSEMENU_ATTEMPTS"), data.Attempts);

            var levelName = _game.CurrentWorld.IsUser ? data.CurrentLevelName : String.Format(Tr(data.CurrentLevelName), _game.CurrentWorld.GetLevelIdx(_game.CurrentLevel) + 1);

            var levelFormatted = $"{Tr(data.CurrentWorldName)} | {levelName}";

            GetNode<Label>("%Attempts").Text = attemptsFormatted;
            GetNode<Label>("%Level").Text = levelFormatted;
            GetNode<Label>("%FragmentCounter").Text = $"{data.FragmentsCollected}/6";
        }

        private Logger _logger = new Logger(nameof(PauseMenu));
        private Game _game;
        private FmodRuntime _fmodRuntime;
    }

    public class PauseMenuData
    {
        public int Attempts = 0;
        public string CurrentWorldName = "";
        public string CurrentLevelName = "";
        public short FragmentsCollected = 0;
    }
}
