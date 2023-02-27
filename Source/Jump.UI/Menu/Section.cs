using Godot;

namespace Jump.UI.Menu
{
    public abstract class MenuSection : UIElement
    {
        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);
            if (@event.IsActionPressed("ui_cancel")) GoBack();
        }

        public override void _Ready() => menu = Owner.GetNode<MenuUI>("%MenuUI");
        public virtual void Focus() { }

        protected void PlayDisplayAnimation() => Visible = true;
        protected void PlayHideAnimation() => Visible = false;
        protected abstract void GoBack();

        protected MenuUI menu;
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