using Godot;

namespace Jump.UI
{
    [Tool]
    public class ButtonMinimal : JAButton
    {
        [Export]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                UpdateLabel(_text);
            }
        }

        [Export]
        public Texture Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                UpdateIcon(_icon, _showIcon);
            }
        }

        [Export]
        public bool ShowIcon
        {
            get => _showIcon;
            set
            {
                _showIcon = value;
                UpdateIcon(_icon, _showIcon);
            }
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            _borders = GetNode<PanelContainer>("%Borders");

            RectPivotOffset = RectSize / 2f;

            _tween = new Tween();
            AddChild(_tween);

            if (!Disabled) return;
            Modulate = new Color(1.0f, 1.0f, 1.0f, 0.1f);
        }

        private void UpdateLabel(string text)
        {
            var label = GetNode<Label>("%Label");
            label.Text = text;
        }

        private void UpdateIcon(Texture iconTexture, bool show = true)
        {
            var icon = GetNode<TextureRect>("%Icon");

            if (!show)
            {
                icon.Visible = false;
                return;
            }

            icon.Visible = true;
            icon.Texture = iconTexture;
        }

        private void PlayPressedSound()
        {
            fmodRuntime.PlayOneShot(buttonPressEvent);
        }

        protected override void OnPressed()
        {
            var tween = CreateTween().SetParallel();
            tween.SetTrans(Tween.TransitionType.Linear);
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "rect_scale", new Vector2(0.9f, 0.9f), tweenTime / 2f);
            tween.Chain().TweenProperty(this, "rect_scale", new Vector2(1f, 1f), tweenTime / 2f);

            PlayPressedSound();
        }

        protected override void OnFocusEntered()
        {
            var tween = _borders.CreateTween().SetParallel();
            tween.SetTrans(Tween.TransitionType.Cubic);
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 1f), tweenTime);
            tween.TweenProperty(this, "rect_position:x", _animationOffsetX, tweenTime);

            fmodRuntime.PlayOneShot(buttonHoverEvent);
        }

        protected override void OnFocusExited()
        {
            var tween = _borders?.CreateTween()?.SetParallel();
            tween?.SetTrans(Tween.TransitionType.Cubic);
            tween?.SetEase(Tween.EaseType.Out);
            tween?.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 0.25f), tweenTime);
            tween?.TweenProperty(this, "rect_position:x", 0f, tweenTime);
        }

        protected override void OnButtonDown()
        {
            PlayButtonDownAnimation();
        }

        protected override void OnButtonUp()
        {
            PlayButtonUpAnimation();
        }

        private void PlayButtonDownAnimation()
        {
            RectPivotOffset = RectSize / 2;

            var initialScale = Vector2.One;
            var targetScale = Vector2.One * 0.8f;

            _tween.InterpolateProperty(this, "rect_scale", initialScale, targetScale, tweenTime, Tween.TransitionType.Quad, Tween.EaseType.Out);
            _tween.Start();
        }

        private void PlayButtonUpAnimation()
        {
            RectPivotOffset = RectSize / 2;

            var initialScale = Vector2.One;

            _tween.InterpolateProperty(this, "rect_scale", RectScale, initialScale, tweenTime, Tween.TransitionType.Back, Tween.EaseType.Out);
            _tween.Start();
        }

        private Tween _tween;
        private PanelContainer _borders;
        private string _text;
        [Export] private float _animationOffsetX = 10f;

        private Texture _icon;
        private bool _showIcon;
    }
}
