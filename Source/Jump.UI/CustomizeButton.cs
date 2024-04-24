using System;
using Godot;

namespace Jump.UI
{
    [Tool]
    public class CustomizeButton : JAButton
    {
        [Export]
        public bool Unlockable
        {
            get => _unlockable; set
            {
                _unlockable = value;
                UpdateElements();
            }
        }
        [Export] public string UnlockId { get => _unlockId; set => _unlockId = value; }
        [Export] public string SkinId { get; set; } = string.Empty;
        [Export] public string ColorId { get; set; } = string.Empty;
        [Export] public string TrailId { get; set; } = string.Empty;
        [Export] public string PressFlavorEvent = string.Empty;

        [Export]
        public Texture PreviewTexture
        {
            get => _previewTexture; set
            {
                _previewTexture = value;
                UpdateElements();
            }
        }
        [Export(PropertyHint.ColorNoAlpha)]
        public Color PreviewModulate
        {
            get => _previewModulate; set
            {
                _previewModulate = value;
                UpdateElements();
            }
        }

        public new Action<CustomizeButton> OnPressedAction;
        public new Action<CustomizeButton> OnFocusEnteredAction;

        public CustomizeButtonState State => _state;

        public override void _Ready()
        {
            base._Ready();
            GetNodes();
        }

        public override void _EnterTree()
        {
            base._EnterTree();
            GetNodes();

            _borderColor = _border.SelfModulate;
            UpdateElements();
        }

        public void PlayUnlockSound()
        {
            fmodRuntime.PlayOneShot("event:/UI/Unlock");
        }

        public void PlayUnlockAnimation()
        {
            var particles = GetNode<Particles2D>("Particles2D");
            particles.Restart();
        }

        public void MakeLocked()
        {
            _state = CustomizeButtonState.Locked;

            var previewColor = _previewModulate;
            previewColor.a = 0.5f;
            _previewModulate = previewColor;

            _lockOverlay.Visible = true;

            UpdateElements();
        }

        public void MakeUnlocked()
        {
            _state = CustomizeButtonState.Unlocked;

            var previewColor = _previewModulate;
            previewColor.a = 1.0f;
            _previewModulate = previewColor;

            _lockOverlay.Visible = false;

            _borderColor = new Color(1f, 1f, 1.0f, 0.5f);

            UpdateElements();
        }

        public void MakeReadyToUnlock()
        {
            _state = CustomizeButtonState.ReadyToUnlock;
            _borderColor = new Color(1f, 1f, 0.0f, 0.5f);

            var previewColor = _previewModulate;
            previewColor.a = 1.0f;
            _previewModulate = previewColor;

            _lockOverlay.Visible = false;

            UpdateElements();
        }

        public void MakeSelected()
        {
            var selectedPanel = GetNode<Panel>("%SelectedPanel");
            selectedPanel.Visible = true;
        }

        public void MakeDeselected()
        {
            var selectedPanel = GetNode<Panel>("%SelectedPanel");
            selectedPanel.Visible = false;
        }

        protected override void OnPressed()
        {
            if (_state == CustomizeButtonState.Locked) return;
            PlayPressSound();

            OnPressedAction?.Invoke(this);
        }

        protected override void OnButtonUp()
        {
            RectPivotOffset = RectSize / 2f;
            var tween = CreateTween().SetParallel();
            tween?.SetTrans(Tween.TransitionType.Quad);
            tween?.SetEase(Tween.EaseType.Out);
            tween?.TweenProperty(this, "rect_scale", new Vector2(1.0f, 1.0f), tweenTime);
        }
        protected override void OnButtonDown()
        {
            if (_state == CustomizeButtonState.Locked) return;
            RectPivotOffset = RectSize / 2f;
            var tween = CreateTween().SetParallel();
            tween?.SetTrans(Tween.TransitionType.Quad);
            tween?.SetEase(Tween.EaseType.Out);
            tween?.TweenProperty(this, "rect_scale", new Vector2(0.8f, 0.8f), tweenTime * 0.5f);
        }

        protected override void OnFocusEntered()
        {
            // if (!_locked) return;
            PlayHoverAnimation();
            PlayHoverSound();

            OnFocusEnteredAction?.Invoke(this);
        }

        protected override void OnFocusExited()
        {
            PlayUnhoverAnimation();
        }

        public void UpdateElements()
        {
            if (!IsInsideTree()) return;

            _previewRect.Texture = _previewTexture;
            _previewRect.SelfModulate = _previewModulate;
            _border.SelfModulate = _borderColor;
        }

        private void GetNodes()
        {
            _border = GetNode<PanelContainer>("%Border");
            _previewRect = GetNode<TextureRect>("%Preview");
            _lockOverlay = GetNode<CenterContainer>("%LockOverlay");
        }

        private void PlayPressSound()
        {
            fmodRuntime.PlayOneShot(buttonPressEvent);

            if (!string.IsNullOrEmpty(PressFlavorEvent))
            {
                fmodRuntime.PlayOneShot(PressFlavorEvent);
            }
        }

        private void PlayHoverSound()
        {
            fmodRuntime.PlayOneShot(buttonHoverEvent);
        }

        private void PlayHoverAnimation()
        {
            var target = _borderColor;
            target.a = 1f;

            var tween = _border.CreateTween().SetParallel();
            tween?.SetTrans(Tween.TransitionType.Linear);
            tween?.SetEase(Tween.EaseType.Out);
            tween?.TweenProperty(_border, "self_modulate", target, tweenTime);
        }

        private void PlayUnhoverAnimation()
        {
            var target = _borderColor;
            target.a = 0.5f;

            var tween = _border.CreateTween().SetParallel();
            tween?.SetTrans(Tween.TransitionType.Linear);
            tween?.SetEase(Tween.EaseType.Out);
            tween?.TweenProperty(_border, "self_modulate", target, tweenTime);
        }

        private bool _unlockable;
        private CustomizeButtonState _state;
        private string _unlockId = string.Empty;
        private PanelContainer _border;
        private TextureRect _previewRect;
        private CenterContainer _lockOverlay;
        private Texture _previewTexture;
        private Color _borderColor = new Color(1f, 1f, 1f, 1f);
        private Color _previewModulate = new Color(1f, 1f, 1f, 1f);
    }

    public enum CustomizeButtonState
    {
        Unlocked,
        Locked,
        ReadyToUnlock
    }
}
