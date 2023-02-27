using Godot;
using Jump.UI.Menu;
using System;

public class AlphaNotice : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _game = GetTree().Root.GetNode<Game>("Game");
        Visible = !_game.DataHandler.Data.AcceptedNotice;

        var paragraphContainer = GetNode<ScrollContainer>("%ParagraphContainer");
        paragraphContainer.GrabFocus();
    }

    private void Accept()
    {
        _game.DataHandler.AcceptNotice();
        _game.DataHandler.SaveData();
        Visible = false;

        var mainSection = GetNode<MainSection>("%MainSection");
        mainSection.Focus();
    }

    private Game _game;
}
