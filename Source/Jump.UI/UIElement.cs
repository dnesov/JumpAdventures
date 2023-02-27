using Godot;

namespace Jump.UI
{
    public abstract class UIElement<T> : UIElement
    {
        public void UpdateElements(T data) => OnUpdateElements(data);
        protected abstract void OnUpdateElements(T data);
    }

    public abstract class UIElement : Control
    {
        public void Display() => OnDisplay();
        public new void Hide() => OnHide();
        public void UpdateElements() => OnUpdateElements();

        protected abstract void OnDisplay();
        protected abstract void OnHide();
        protected virtual void OnUpdateElements() { }
    }

    public class UIData { }
}