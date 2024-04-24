using Godot;

using Jump.Utils;
using Jump.Unlocks;
using Jump.Extensions;

namespace Jump.Entities
{
    public class VaultDoor : Area2D, IInteractable
    {
        public VaultDoor()
        {

        }

        [Signal] public delegate void OnUnlockEventHandler();

        public override void _Ready()
        {
            base._Ready();
            GetNodes();
        }

        private void GetNodes()
        {
            _doorBody = GetNode<StaticBody2D>("StaticBody2D");

            _tween = new Tween();
            AddChild(_tween);

            _unlockDb = this.GetSingleton<UnlocksDatabase>();
            _unlockable = _unlockDb.GetUnlockable(_unlockableId);
        }

        private void Open()
        {
            PlayOpenAnimation();
            _opened = true;

            _unlockDb.TryUnlock(_unlockableId);
        }

        private void Close()
        {
            PlayCloseAnimation();
            _opened = false;
        }

        private bool CanOpen()
        {
            return _unlockable == null || _unlockable.CanUnlock();
        }

        private void PlayOpenAnimation()
        {
            _tween.RemoveAll();
            _tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
            _tween.InterpolateProperty(_doorBody, "position:y", _doorBody.Position.y, _targetY, _tweenDuration, Tween.TransitionType.Quad, Tween.EaseType.InOut);
            _tween.Start();
        }

        private void PlayCloseAnimation()
        {
            _tween.RemoveAll();
            _tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
            _tween.InterpolateProperty(_doorBody, "position:y", _doorBody.Position.y, 0, _tweenDuration, Tween.TransitionType.Bounce, Tween.EaseType.Out);
            _tween.Start();
        }

        public string GetInteractMessage()
        {
            if (!CanOpen())
            {
                return _lockedMessage;
            }

            if (!_opened)
            {
                return "Press [b]E[/b] to unlock this Vault.";
            }

            return string.Empty;
        }

        public void OnInteract()
        {
            if (CanOpen() && !_opened)
            {
                Open();
            }
        }

        private UnlocksDatabase _unlockDb;
        [Export] private string _unlockableId = "";
        [Export] private float _targetY = -320f;
        [Export] private float _tweenDuration = 1f;
        [Export] private string _lockedMessage;
        private StaticBody2D _doorBody;
        private UnlockableBase _unlockable;
        private Tween _tween;

        private bool _opened;
    }
}