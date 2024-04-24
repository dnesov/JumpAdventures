using Godot;
using GodotFmod;
using Jump.Extensions;
using Jump.Misc;
using Jump.Utils;
using System;
using System.Collections.Generic;

namespace Jump.UI
{
    public class LevelCompleteUI : UIElement<LevelCompleteUIData>
    {
        public override void _Ready()
        {
            base._Ready();
            GetNodes();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
        }

        public void LoadNextLevel()
        {
            if (_displayingAnimation)
            {
                _attemptsStat.PauseTween();
                _essenceStat.PauseTween();
                _timeStat.PauseTween();

                _displayingAnimation = false;
            }

            var nextButton = GetNode<LevelCompleteButton>("%NextLevel");
            var backButton = GetNode<LevelCompleteButton>("%Back");

            nextButton.Disabled = true;
            backButton.Disabled = true;

            Hide();

            _game.LoadNextLevel();
        }

        public void GoToMenu()
        {
            if (_displayingAnimation)
            {
                _attemptsStat.PauseTween();
                _essenceStat.PauseTween();
                _timeStat.PauseTween();

                _displayingAnimation = false;
            }

            var nextButton = GetNode<LevelCompleteButton>("%NextLevel");
            var backButton = GetNode<LevelCompleteButton>("%Back");

            nextButton.Disabled = true;
            backButton.Disabled = true;

            Hide();

            _game.ReturnToMenu(true);
        }

        protected override void OnDisplay()
        {
            Visible = true;
            Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            RectPosition = new Vector2(RectPosition.x, 200);

            PlayDisplayAnimation();

            var nextLevelButton = GetNode<LevelCompleteButton>("%NextLevel");
            nextLevelButton.GrabFocus();

            _game.EnableCursor();
        }

        private void GetNodes()
        {
            _game = this.GetSingleton<Game>();

            _attemptsStat = GetNode<LevelCompleteIntStat>("%Attempts");
            _essenceStat = GetNode<LevelCompleteIntStat>("%Essence");
            _timeStat = GetNode<LevelCompleteFloatStat>("%Time");

            // _leaderboardUi = GetNode<LeaderboardUI>("%LeaderboardUI");
        }

        private void PlayDisplayAnimation()
        {
            Visible = true;
            var tween = CreateTween();
            tween.TweenInterval(0.5f);
            tween.SetParallel();
            tween.Chain().TweenCallback(this, nameof(PlayDisplaySound));
            tween.Chain().TweenProperty(this, "modulate", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.55f);
            tween.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "rect_position", new Vector2(0, 0), 0.35f);
            tween.Chain().TweenCallback(this, nameof(PlayStatAnimations));
        }

        private void PlayHideAnimation()
        {
            var tween = CreateTween();
            tween.Parallel().TweenProperty(this, "modulate", new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.55f);
            tween.Parallel().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out).TweenProperty(this, "rect_position", new Vector2(0, 200), 0.35f);
            tween.Chain().TweenCallback(this, "hide");
        }

        protected override void OnHide()
        {
            PlayHideAnimation();
        }

        protected override void OnUpdateElements(LevelCompleteUIData data)
        {
            _displayingAnimation = true;
            _data = data;
        }

        private void PlayDisplaySound()
        {
            var fmodRuntime = this.GetSingleton<FmodRuntime>();
            fmodRuntime.PlayOneShot("event:/LevelCompleteDisplay");
        }

        private async void PlayStatAnimations()
        {
            // await this.TimeInSeconds(0.25f);
            if (!_displayingAnimation) return;

            _attemptsStat.TargetValue = _data.Attempts;
            _attemptsStat.Display();

            await ToSignal(_attemptsStat.Tweener, "finished");
            await this.TimeInSeconds(0.25f);

            if (!_displayingAnimation) return;

            _essenceStat.TargetValue = _data.EssenceCollected;
            _essenceStat.Display();

            await ToSignal(_essenceStat.Tweener, "finished");
            await this.TimeInSeconds(0.25f);

            if (!_displayingAnimation) return;

            _timeStat.TargetValue = _data.Time;
            _timeStat.Display();

            _displayingAnimation = false;
        }

        private LevelCompleteIntStat _attemptsStat, _essenceStat;
        private LevelCompleteFloatStat _timeStat;
        private Game _game;
        private bool _displayingAnimation;

        // private LeaderboardUI _leaderboardUi;
        private LevelCompleteUIData _data;
    }

    public class LevelCompleteUIData : UIData
    {
        public bool IsChallengeMode;
        public int Attempts;
        public int EssenceCollected;
        public int Experience;
        public int MaxExperience;
        public float Time;
        public float HeartsBonus;
        public float EssenceBonus;
        public float TotalBonus;
    }
}