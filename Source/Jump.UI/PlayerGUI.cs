using Godot;
using Jump.Entities;
using Jump.Extensions;
using Jump.Misc;
using System;

namespace Jump.UI
{
    public class PlayerGUI : CanvasLayer
    {
        public MessageUI MessageUI => _messageUI;

        // Called when the node enters the scene tree for the first time.
        public override async void _Ready()
        {
            GetNodes();
            SubscribeEvents();

            await ToSignal(_player, "ready");

            _healthContainer.PopulateHealthSprites(_player.HealthHandler.Hearts, _player.HealthHandler.MaxHearts);
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        public void DisplayMessage(string message, float duration) => _messageUI.DisplayMessage(message, duration);

        private async void SubscribeEvents()
        {
            _game.OnWin += OnWin;
            _game.OnHideUi += OnHideUi;
            _game.OnShowUi += OnShowUi;
            _game.OnRetry += OnRetry;

            _player.OnRetryRequested += OnRetryRequested;
            _transitionUI.OnCanRespawn += OnCanRespawn;

            await ToSignal(_player, "ready");

            _player.HealthHandler.OnDeath += OnDeath;
        }

        private void UnsubscribeEvents()
        {
            _game.OnWin -= OnWin;
            _game.OnHideUi -= OnHideUi;
            _game.OnShowUi -= OnShowUi;
            _game.OnRetry -= OnRetry;

            _player.OnRetryRequested -= OnRetryRequested;
            _player.HealthHandler.OnDeath -= OnDeath;

            _transitionUI.OnCanRespawn -= OnCanRespawn;
        }

        private void OnHideUi() => Visible = false;
        private void OnShowUi() => Visible = true;

        private void OnWin()
        {
            GameModeBase gamemode = _game.CurrentGameMode;
            LevelCompleteUIData data;

            if (gamemode is ChallengeGameMode challengeGameMode)
            {
                var heartsBonus = challengeGameMode.CalculateHeartsBonus(_player.HealthHandler.Hearts);
                var essenceBonus = challengeGameMode.CalculateCollectibleBonus(_player.LevelRoot.MaxEssence);
                // var experience = challengeGameMode.CalculateExperience(heartsBonus, essenceBonus, _game.CurrentLevel.MaxExperience);

                data = new LevelCompleteUIData()
                {
                    Attempts = _game.Attempts,
                    EssenceCollected = _game.CurrentGameMode.Essence,
                    Time = _game.Timer,
                    IsChallengeMode = _game.CurrentGameMode is ChallengeGameMode,
                    HeartsBonus = heartsBonus,
                    EssenceBonus = essenceBonus,
                    TotalBonus = challengeGameMode.CalculateBonus(heartsBonus, essenceBonus),
                    // Experience = experience,
                    // MaxExperience = _game.CurrentLevel.MaxExperience
                };
            }
            else
            {
                data = new LevelCompleteUIData()
                {
                    Attempts = _game.Attempts,
                    EssenceCollected = _game.CurrentGameMode.Essence,
                    Time = _game.Timer,
                };
            }

            _levelCompleteUI.UpdateElements(data);
            _levelCompleteUI.Display();
        }

        private void OnRetryRequested()
        {
            // TODO: I think this should be in Player, but it will stay here for now.
            var incrementAttempts = _player.ShouldIncrementAttempts();
            _game.TryRetry(incrementAttempts);
        }

        private void GetNodes()
        {
            _game = this.GetSingleton<Game>();
            _player = GetParent<Player>();
            _messageUI = GetNode<MessageUI>("%MessageUI");
            _levelCompleteUI = GetNode<LevelCompleteUI>("%LevelCompleteUI");
            _transitionUI = GetNode<TransitionUI>("%TransitionUI");
            _healthContainer = GetNode<HealthContainer>("%HealthContainer");
            _restartContainer = GetNode<Control>("%RestartContainer");
        }

        private void OnDeath()
        {
            var restartLabel = _restartContainer.GetNode<RestartLabel>("RestartLabel");
            _restartContainer.Show();
            restartLabel.UpdateElements();

            // if (_restartTween != null && _restartTween.IsRunning())
            // {
            //     _restartTween.Stop();
            // }

            _restartTween = _restartContainer.CreateTween();
            _restartTween.TweenProperty(_restartContainer, "modulate:a", 1.0f, 0.25f);
        }

        private void OnRetry()
        {
            DoRetryTransition();

            if (_game.CurrentState == GameState.PlayingOverWin)
            {
                _levelCompleteUI.Hide();
            }

            // if (_restartTween != null && _restartTween.IsRunning())
            // {
            //     _restartTween.Stop();
            // }

            if (_game.CurrentState == GameState.PlayingOver)
            {
                _restartTween = _restartContainer.CreateTween();
                _restartTween.TweenProperty(_restartContainer, "modulate:a", 0.0f, 0.25f);
                _restartTween.TweenCallback(_restartContainer, "hide");
            }
        }
        private void DoRetryTransition() => _transitionUI.Display();
        private void OnCanRespawn()
        {
            if (_restartContainer.Visible)
            {
                _restartContainer.Hide();
            }

            _player.Respawn();
            _game.LateRetry();
        }

        private SceneTreeTween _restartTween;
        private Player _player;
        private Game _game;
        private MessageUI _messageUI;
        private LevelCompleteUI _levelCompleteUI;
        private TransitionUI _transitionUI;
        private HealthContainer _healthContainer;
        private Control _restartContainer;
    }
}