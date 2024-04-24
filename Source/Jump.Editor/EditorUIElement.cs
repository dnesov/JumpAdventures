using Jump.UI;

namespace Jump.Editor
{
    public class EditorUIElement : UIElement
    {
        public void RegisterEditor(Editor editor)
        {
            this.editor = editor;
        }

        protected override void OnDisplay()
        {
            Show();
        }

        protected override void OnHide()
        {
            Visible = false;
        }

        protected Editor editor;
    }
}