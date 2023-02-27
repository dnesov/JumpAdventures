using Godot;
using Jump.Unlocks;
using Jump.Utils;
using System;

namespace Jump.UI
{
    public class CustomizeButton : JAButton
    {
        public new Action<string, string> OnPressedAction;
        public Action<bool, string> OnFocusedAction;

        public override void _Ready()
        {
            base._Ready();
            _preview = GetNode<TextureRect>("%Preview");
            _borders = GetNode<PanelContainer>("%Border");

            _database = GetTree().Root.GetNode<UnlocksDatabase>("UnlocksDatabase");

            _borderColor = _borders.SelfModulate;
        }

        public void UpdateElements(CustomizeButtonData data)
        {
            if (data.PreviewTexture != null) _preview.Texture = data.PreviewTexture;
            _preview.SelfModulate = data.PreviewModulate;

            _skinId = data.SkinId;
            _colorId = data.ColorId;
            _unlockId = data.UnlockId;

            var isUnlocked = _database.IsUnlocked(_unlockId);

            var unlock = _database.GetUnlockable(_unlockId);
            if (unlock != null && !isUnlocked && unlock.CanUnlock())
                SetReadyToUnlock();


            if (isUnlocked) SetUnlocked();
            else SetLocked();
        }

        private void SetUnlocked()
        {
            var modulate = _preview.SelfModulate;
            modulate.a = 1.0f;
            _preview.SelfModulate = modulate;
            GetNode<CenterContainer>("%LockOverlay").Visible = false;

            _borderColor = new Color(1f, 1f, 1f, 0.4f);
            _borders.SelfModulate = _borderColor;
        }
        private void SetLocked()
        {
            var modulate = _preview.SelfModulate;
            modulate.a = 0.4f;
            _preview.SelfModulate = modulate;
            GetNode<CenterContainer>("%LockOverlay").Visible = true;
        }

        private void SetReadyToUnlock()
        {
            _borderColor = new Color(0.9f, 1f, 0f, 0.4f);
            _borders.SelfModulate = _borderColor;
        }

        private bool IsUnlocked()
        {
            return _database.IsUnlocked(_unlockId);
        }

        protected override void OnFocusEntered()
        {
            var isUnlocked = _database.IsUnlocked(_unlockId);
            var unlockable = _database.GetUnlockable(_unlockId);
            var focusMessage = unlockable == null || isUnlocked ? "" : unlockable.FormattedDescription;
            OnFocusedAction?.Invoke(IsUnlocked(), focusMessage);
            PlayHoverAnimation();
            fmodRuntime.PlayOneShot(buttonHoverEvent);
        }

        protected override void OnFocusExited()
        {
            PlayUnhoverAnimation();
        }

        protected override void OnPressed()
        {
            if (_unlockId != string.Empty)
            {
                var unlock = _database.GetUnlockable(_unlockId);
                if (unlock == null) return;
                if (!unlock.CanUnlock()) return;
                if (!_database.IsUnlocked(_unlockId)) fmodRuntime.PlayOneShot(UNLOCK_EVENT);
                _database.TryUnlock(_unlockId);
                SetUnlocked();
            }

            PlayPressAnimation();
            fmodRuntime.PlayOneShot(buttonPressEvent);
            OnPressedAction?.Invoke(_skinId, _colorId);
        }

        private void PlayPressAnimation()
        {
            RectPivotOffset = RectSize / 2f;
            var tween = CreateTween().SetParallel();
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "rect_scale", new Vector2(0.8f, 0.8f), tweenTime * 0.5f);
            tween.SetTrans(Tween.TransitionType.Circ);
            tween.Chain().TweenProperty(this, "rect_scale", new Vector2(1f, 1f), tweenTime * 0.9f);
        }

        private void PlayHoverAnimation()
        {
            var target = new Color(_borderColor.r, _borderColor.g, _borderColor.b, 1f);
            var tween = _borders.CreateTween().SetParallel();
            tween.SetTrans(Tween.TransitionType.Linear);
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(_borders, "self_modulate", target, tweenTime);
        }

        private void PlayUnhoverAnimation()
        {
            var target = new Color(_borderColor.r, _borderColor.g, _borderColor.b, 0.4f);
            var tween = _borders.CreateTween().SetParallel();
            tween?.SetTrans(Tween.TransitionType.Linear);
            tween?.SetEase(Tween.EaseType.Out);
            tween?.TweenProperty(_borders, "self_modulate", target, tweenTime);
        }

        private Color _borderColor;
        private TextureRect _preview;
        private string _skinId, _colorId, _unlockId;
        private PanelContainer _borders;
        private UnlocksDatabase _database;
        private const string UNLOCK_EVENT = "event:/UI/Unlock";
    }

    public class CustomizeButtonData : UIData
    {
        public string SkinId;
        public string ColorId;
        public string UnlockId;
        public Texture PreviewTexture;
        public Color PreviewModulate = new Color(1f, 1f, 1f, 1f);
    }
}
