using FMOD;
using Godot;
using Jump.Extensions;
using Jump.Utils;

namespace Jump.UI.Menu
{
    public abstract class MenuSection : UIElement
    {
        [Signal] public delegate void OnBack();
        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);
            if (@event.IsActionPressed("ui_cancel")) GoBack();
        }

        public override void _Ready()
        {
            SetProcessInput(false);
            SetProcessUnhandledInput(false);

            menu = Owner.GetNodeOrNull<MenuUI>("%MenuUI");

            soundtrackManager = this.GetSingleton<SoundtrackManager>();
            ResetVisuals();
        }

        public virtual void Focus() { }

        public void GoBack()
        {
            EmitSignal(nameof(OnBack));
            OnGoBack();
        }

        protected void PlayDisplayAnimation()
        {
            Show();

            if (_tweener is not null && _tweener.IsRunning())
            {
                _tweener.Kill();
            }

            _tweener = CreateTween();
            _tweener.SetParallel();
            _tweener.SetTrans(_transitionType);
            _tweener.SetEase(_easeType);
            _tweener.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 1f), _animationTime);
            _tweener.TweenProperty(this, "rect_position:x", _initialPosX, _animationTime);
        }
        protected void PlayHideAnimation()
        {
            if (_tweener is not null && _tweener.IsRunning())
            {
                _tweener.Kill();
            }

            _tweener = CreateTween();
            _tweener.SetParallel();
            _tweener.SetTrans(_transitionType);
            _tweener.SetEase(_easeType);
            _tweener.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 0f), _animationTime);

            if (menu != null)
            {
                _tweener.TweenProperty(this, "rect_position:x", _initialPosX - _xOffset, _animationTime);
            }
            else
            {
                _tweener.TweenProperty(this, "rect_position:x", _initialPosX + _xOffset, _animationTime);
            }

            _tweener.Chain().TweenCallback(this, nameof(SetInvisible));
        }

        protected abstract void OnGoBack();

        protected override void OnDisplay()
        {
            SetProcessInput(true);
            SetProcessUnhandledInput(true);

            var hasBackButton = HasNode("%Back");
            if (!hasBackButton) return;
            var backButton = GetNode<ButtonMinimal>("%Back");
            backButton.OnPressedAction += GoBack;
        }
        protected override void OnHide()
        {
            SetProcessInput(false);
            SetProcessUnhandledInput(false);

            var hasBackButton = HasNode("%Back");
            if (!hasBackButton) return;
            var backButton = GetNode<ButtonMinimal>("%Back");
            backButton.OnPressedAction -= GoBack;
        }

        private void ResetVisuals()
        {
            _initialPosX = RectPosition.x;
            Modulate = new Color(1f, 1f, 1f, 0.0f);

            var rectPos = RectPosition;
            rectPos.x = _initialPosX - _xOffset;
            RectPosition = rectPos;
        }

        private void SetInvisible() => Visible = false;

        protected MenuUI menu;
        private float _animationTime = 0.45f;
        private Tween.TransitionType _transitionType = Tween.TransitionType.Cubic;
        private Tween.EaseType _easeType = Tween.EaseType.Out;
        private float _xOffset = 100f;
        private float _initialPosX;

        private SceneTreeTween _tweener;

        protected SoundtrackManager soundtrackManager;
    }
    public abstract class MenuSection<T> : UIElement<T>
    {
        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);
            if (@event.IsActionPressed("ui_cancel")) GoBack();
        }

        public override void _Ready() => menu = Owner.GetNode<MenuUI>("%MenuUI");
        protected void PlayDisplayAnimation() => Visible = true;
        protected void PlayHideAnimation() => Visible = false;
        protected abstract void GoBack();
        protected MenuUI menu;
    }

    public abstract class Section : UIElement
    {
        protected void PlayDisplayAnimation() => Visible = true;
        protected void PlayHideAnimation() => Visible = false;
    }

    public class SectionData : UIData { }
}