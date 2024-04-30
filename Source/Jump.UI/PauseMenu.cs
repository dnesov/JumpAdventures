using System;
using System.Collections.Generic;
using Godot;
using GodotFmod;
using Jump.Extensions;
using Jump.UI.Menu;
using Jump.Utils;

namespace Jump.UI
{
    public class PauseMenu : UIElement<PauseMenuData>
    {
        public override void _Ready()
        {
            _game = this.GetSingleton<Game>();
            _fmodRuntime = this.GetSingleton<FmodRuntime>();

            _game.OnPaused += OnPaused;
            _game.OnResumed += OnResumed;

            Hide();
        }

        public override void _ExitTree()
        {
            _game.OnPaused -= OnPaused;
            _game.OnResumed -= OnResumed;
        }

        public override void _Input(InputEvent @event)
        {
            var settingsSection = GetNode<SettingsSection>("%PauseSettingsSection");

            if (!Visible || settingsSection.Visible) return;

            if (@event.IsActionPressed("ui_cancel") && _game.LastInputMethod == InputMethod.Controller)
            {
                Resume();
            }
        }

        private void OnPaused() => Display();
        private void OnResumed()
        {
            Hide();
            PlayHideSound();
        }
        private void Resume() => _game.Resume();

        private void PlayDisplayAnimation()
        {
            var overlay = GetNode<ColorRect>("%Overlay");
            var mainNav = GetNode<VBoxContainer>("%MainNav");

            if (_tweener is not null && _tweener.IsRunning())
            {
                _tweener.Kill();
            }

            _tweener = CreateTween().SetParallel();
            _tweener.SetTrans(Tween.TransitionType.Cubic);
            _tweener.SetEase(Tween.EaseType.Out);
            _tweener.Chain().TweenCallback(this, "show");
            _tweener.TweenProperty(overlay, "modulate", new Color(1f, 1f, 1f, 1f), 0.3f);
            _tweener.TweenProperty(mainNav, "rect_position", new Vector2(80, mainNav.RectPosition.y), 0.4f);
            _tweener.TweenProperty(mainNav, "modulate", new Color(1f, 1f, 1f, 1f), 0.4f);
            Visible = true;
        }

        private void PlayHideAnimation()
        {
            var overlay = GetNode<ColorRect>("%Overlay");
            var mainNav = GetNode<VBoxContainer>("%MainNav");

            if (_tweener is not null && _tweener.IsRunning())
            {
                _tweener.Kill();
            }

            _tweener = CreateTween().SetParallel();
            _tweener.SetTrans(Tween.TransitionType.Cubic);
            _tweener.SetEase(Tween.EaseType.Out);
            _tweener.TweenProperty(overlay, "modulate", new Color(1f, 1f, 1f, 0f), 0.3f);
            _tweener.TweenProperty(mainNav, "rect_position", new Vector2(-200, mainNav.RectPosition.y), 0.5f);
            _tweener.TweenProperty(mainNav, "modulate", new Color(1f, 1f, 1f, 0f), 0.4f);
            _tweener.Chain().TweenCallback(this, "hide");
        }

        private void PlayDisplaySound()
        {
            _fmodRuntime.PlayOneShot("event:/UI/SectionClick");
            _fmodRuntime.GetBus("bus:/SFX").Paused = true;
            _fmodRuntime.SetParameter("Filter", 1f);
        }

        private void PlayHideSound()
        {
            _fmodRuntime.PlayOneShot("event:/UI/SectionLeave");
        }

        private void MainMenu()
        {
            Resume();
            _game.ReturnToMenu(true);
            _logger.Info("Switching to main menu.");
        }

        private void OpenFeedbackForm()
        {
            // Constants.OpenFeedbackForm(_game.CurrentLevelString);
            GD.PushError("Feedback forms are disabled for this build!");
        }

        // private async void ShowLeaderboard()
        // {
        //     var leaderboard = GetNode<LeaderboardUI>("%LeaderboardUI");

        //     leaderboard.Display();
        //     leaderboard.Loading = true;

        //     if (_game.Campaign)
        //     {
        //         var leaderboardId = _game.CurrentLevelString;

        //         var entries = await _game.LeaderboardProvider.GetEntriesAsync(leaderboardId);

        //         if (entries == null)
        //         {
        //             entries = new List<LeaderboardEntry>();
        //         }

        //         var data = new LeaderboardData
        //         {
        //             Entries = entries,
        //             CanUpload = false
        //         };

        //         leaderboard.Loading = false;
        //         leaderboard.UpdateElements(data);
        //     }
        //     else
        //     {
        //         leaderboard.Loading = false;
        //         leaderboard.Hide();
        //         return;
        //     }
        // }

        private void ShowSettings()
        {
            var settingsSection = GetNode<SettingsSection>("%PauseSettingsSection");
            settingsSection.Display();
        }
        private void HideSettings()
        {
            var settingsSection = GetNode<SettingsSection>("%PauseSettingsSection");
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
                FragmentsCollected = _game.CurrentWorldSaveData.FragmentsCollected,
                GameModeName = _game.CurrentGameMode.Name
            });

            _game.EnableCursor();
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

            _game.DisableCursor();
        }

        protected override void OnUpdateElements(PauseMenuData data)
        {
            var attemptsFormatted = string.Format(Tr("UI_PAUSEMENU_ATTEMPTS"), data.Attempts);
            var levelName = _game.CurrentWorld.IsUser ? data.CurrentLevelName : string.Format(Tr(data.CurrentLevelName), _game.CurrentWorld.GetLevelIdx(_game.CurrentLevel) + 1);

            GetNode<Label>("%Attempts").Text = attemptsFormatted;
            GetNode<Label>("%WorldName").Text = data.CurrentWorldName;
            GetNode<Label>("%LevelName").Text = levelName;
            GetNode<Label>("%GameModeName").Text = data.GameModeName;
            GetNode<Label>("%FragmentCounter").Text = $"{data.FragmentsCollected}/6";
        }

        private Logger _logger = new Logger(nameof(PauseMenu));
        private Game _game;
        private FmodRuntime _fmodRuntime;

        private SceneTreeTween _tweener;
    }

    public class PauseMenuData
    {
        public int Attempts = 0;
        public string CurrentWorldName = "";
        public string CurrentLevelName = "";
        public string GameModeName = "";
        public short FragmentsCollected = 0;
    }
}
