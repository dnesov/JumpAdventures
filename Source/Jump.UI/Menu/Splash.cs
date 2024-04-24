using Godot;
using Jump.Extensions;
using System;

public class Splash : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var game = this.GetSingleton<Game>();

        if (game.SplashPlayed)
            Visible = false;
        else
        {
            var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            animationPlayer.Play("appear");
            game.SplashPlayed = true;
        }
    }
}
