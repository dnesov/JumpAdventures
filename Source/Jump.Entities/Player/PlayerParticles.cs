using Godot;
using System;

namespace Jump.Entities
{
    public class PlayerParticles : Particles2D
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            SetAsToplevel(true);
            ShowBehindParent = true;
            _sprite = GetParent<PlayerSprite>();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            GlobalPosition = _sprite.GlobalPosition;
        }

        private PlayerSprite _sprite;
    }
}
