using Godot;
using Jump.Entities;
using System;

namespace Jump.UI
{
    public class PlayerGUI : CanvasLayer
    {
        public MessageUI MessageUI => _messageUI;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GetNodes();
            SubscribeEvents();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _game.OnWin += OnWin;
            _game.OnHideUi += OnHideUi;
            _game.OnShowUi += OnShowUi;
            // _game.OnRetry += OnRetry;
            _player.OnRetryRequested += OnRetry;
        }

        private void UnsubscribeEvents()
        {
            _game.OnWin -= OnWin;
            _game.OnHideUi -= OnHideUi;
            _game.OnShowUi -= OnShowUi;
            // _game.OnRetry -= OnRetry;
            _player.OnRetryRequested -= OnRetry;
        }

        private void OnHideUi() => Visible = false;
        private void OnShowUi() => Visible = true;

        private void OnWin()
        {
            _levelCompleteUI.UpdateElements(new LevelCompleteUIData()
            {
                Attempts = _game.Attempts,
                EssenceCollected = _game.SessionEssence,
            });
            _levelCompleteUI.Display();
        }

        private void OnRetry()
        {
            DoRetryTransition();
        }

        private void GetNodes()
        {
            _game = GetTree().Root.GetNode<Game>("Game");
            _player = GetParent<Player>();
            _messageUI = GetNode<MessageUI>("%MessageUI");
            _levelCompleteUI = GetNode<LevelCompleteUI>("%LevelCompleteUI");
            _transitionUI = GetNode<TransitionUI>("%TransitionUI");

            _transitionUI.OnCanRespawn += OnCanRespawn;
        }

        private void DoRetryTransition()
        {
            _transitionUI.Display();
        }

        private void OnCanRespawn()
        {
            _player.RespawnAndRetry();
        }

        private Player _player;
        private Game _game;
        private MessageUI _messageUI;
        private LevelCompleteUI _levelCompleteUI;
        private TransitionUI _transitionUI;
    }
}