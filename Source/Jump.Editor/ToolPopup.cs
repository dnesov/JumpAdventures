using Godot;
using System;

namespace Jump.Editor
{
    public class ToolPopup : EditorUIElement
    {
        private void AddPlayerPressed()
        {
            var player = editor.AddPlayerAt(RectGlobalPosition);

            var addPlayerButton = GetNode<Button>(_addPlayerButtonPath);
            addPlayerButton.Disabled = true;

            Hide();
        }

        [Export] private NodePath _addPlayerButtonPath;
    }
}