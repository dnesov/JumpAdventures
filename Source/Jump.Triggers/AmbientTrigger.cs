using Godot;
using Jump.Entities;
using Jump.Extensions;
using Jump.Utils;
using System;

namespace Jump.Triggers
{
    public class AmbientTrigger : BaseTrigger
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _soundtrackManager = this.GetSingleton<SoundtrackManager>();
        }

        protected override void OnEntered(Player player)
        {
            _soundtrackManager.PlayAmbience(_enterEventPath);
        }

        protected override void OnExited(Player player)
        {
            _soundtrackManager.PlayAmbience(_exitEventPath);
        }

        [Export] private string _enterEventPath;
        [Export] private string _exitEventPath;
        private SoundtrackManager _soundtrackManager;
    }
}
