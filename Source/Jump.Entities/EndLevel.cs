using Godot;
using GodotFmod;
using System;

namespace Jump.Entities
{
    public class EndLevel : Area2D, IObstacle
    {
        public override void _Ready()
        {
            base._Ready();
            _fmodRuntime = GetTree().Root.GetNode<FmodRuntime>("FmodRuntime");
        }
        public void PlayerEntered(Player player)
        {
            var game = GetTree().Root.GetNode<Game>("Game");
            game.Win();
            _fmodRuntime.PlayOneShot("event:/Win");
        }

        public void PlayerExited(Player player) { }

        private FmodRuntime _fmodRuntime;
    }
}
