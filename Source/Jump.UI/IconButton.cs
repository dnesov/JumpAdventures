using Godot;
using System;

public class IconButton : TextureButton
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("mouse_entered", this, nameof(MouseEntered));
        Connect("mouse_exited", this, nameof(MouseExited));
        Connect("focus_entered", this, nameof(FocusEntered));
        Connect("focus_exited", this, nameof(FocusExited));

        Modulate = new Color(1f, 1f, 1f, 0.8f);
    }

    private void MouseEntered()
    {
        GrabFocus();
    }

    private void MouseExited()
    {
        ReleaseFocus();
    }

    private void FocusEntered()
    {
        RectPivotOffset = RectScale / 2;
        var tween = CreateTween();
        tween.SetParallel();
        tween.TweenProperty(this, "modulate:a", 1.0f, 0.1f);
        tween.TweenProperty(this, "rect_scale", Vector2.One * 1.05f, 0.1f);
    }

    private void FocusExited()
    {
        RectPivotOffset = RectScale / 2;
        var tween = CreateTween();
        tween.SetParallel();
        tween.TweenProperty(this, "modulate:a", 0.8f, 0.1f);
        tween.TweenProperty(this, "rect_scale", Vector2.One, 0.1f);
    }
}
