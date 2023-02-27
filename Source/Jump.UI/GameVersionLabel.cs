using Godot;
using System;

public class GameVersionLabel : Label
{
    public override void _Ready()
    {
        Text = Constants.VersionFull;
    }
}
