using Godot;
using System;

public class LevelSelectButton : Button
{
    public LevelSelectButton(short levelId)
    {
        _levelId = levelId;
    }

    public Action<short> OnPressed;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("pressed", this, nameof(ButtonPressed));
    }

    private void ButtonPressed()
    {
        OnPressed.Invoke(_levelId);
    }

    private short _levelId;
}
