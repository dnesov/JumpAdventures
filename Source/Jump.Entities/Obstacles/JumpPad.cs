using Godot;
using GodotFmod;
using Jump.Extensions;
using System;

namespace Jump.Entities
{
    [Tool]
    public class JumpPad : ObstacleBase
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            _fmodRuntime = this.GetSingleton<FmodRuntime>();
        }

        protected override void OnPlayerEntered(Player player)
        {
            player.Jump(_jumpMultiplier, false);
            _fmodRuntime.PlayOneShot("event:/JumpPad");

            var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            animationPlayer.Play("triggered");
        }

        protected override void OnPlayerExited(Player player)
        {

        }

        private FmodRuntime _fmodRuntime;
        [Export] private float _jumpMultiplier = 1.5f;
    }

}