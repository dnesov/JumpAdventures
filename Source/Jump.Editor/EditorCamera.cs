using Godot;
using System;

namespace Jump.Editor
{
    public class EditorCamera : Camera2D
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Current = true;
        }

        public override void _Input(InputEvent @event)
        {
            // GD.Print("inputting");
            if (@event is InputEventMouseMotion mouse)
            {
                if (mouse.ButtonMask == 4)
                {
                    Position -= mouse.Relative * Zoom;
                }
            }

            if (@event is InputEventMouseButton mouseButton)
            {
                if (mouseButton.Pressed)
                {
                    if (mouseButton.ButtonIndex == (int)ButtonList.WheelUp)
                    {
                        ZoomIn();
                    }

                    if (mouseButton.ButtonIndex == (int)ButtonList.WheelDown)
                    {
                        ZoomOut();
                    }
                }
            }
        }

        private void ZoomIn()
        {
            _targetZoom = Mathf.Max(_targetZoom - 0.1f, 0.1f);
            Zoom = Vector2.One * _targetZoom;
        }

        private void ZoomOut()
        {
            _targetZoom = Mathf.Min(_targetZoom + 0.1f, 2f);
            Zoom = Vector2.One * _targetZoom;
        }

        private float _targetZoom = 1.0f;
    }
}