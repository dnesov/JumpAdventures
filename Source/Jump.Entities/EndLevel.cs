using Godot;
using GodotFmod;
using Jump.Extensions;
using System;

namespace Jump.Entities
{
    public class EndLevel : Area2D, IObstacle
    {
        public override void _Ready()
        {
            base._Ready();
            _fmodRuntime = this.GetSingleton<FmodRuntime>();
        }
        public void PlayerEntered(Player player)
        {
            var game = this.GetSingleton<Game>();
            game.Win();
            _fmodRuntime.PlayOneShot("event:/Win");
        }

        public void PlayerExited(Player player) { }

        public void Enable()
        {
            Visible = true;
            Monitoring = true;
        }

        public void Disable()
        {
            Visible = false;
            Monitoring = false;
        }

        private FmodRuntime _fmodRuntime;
    }
}
