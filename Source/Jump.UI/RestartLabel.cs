using Godot;
using Jump.Extensions;
using System;

namespace Jump.UI;

public class RestartLabel : RichTextLabel
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _game = this.GetSingleton<Game>();
    }

    public void UpdateElements()
    {
        var translated = Tr(_translationString);
        var usingKeyboard = _game.LastInputMethod == InputMethod.Keyboard;
        var formatted = $"[center]{string.Format(translated, usingKeyboard ? _keyboardFormat : _xboxFormat)}[/center]";

        BbcodeText = formatted;
    }

    private Game _game;

    [Export] private string _translationString = "UI_GAME_RESTART";
    [Export] private string _keyboardFormat = "[b]R[/b]";
    [Export] private string _xboxFormat = "[img=24]res://Assets/UI/ButtonPrompts/xbox_x.png[/img]";
}
