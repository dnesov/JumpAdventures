using Godot;
using Jump.Entities;
using Jump.Utils;
using System;

namespace Jump.Triggers
{
    public class AmbientTrigger : BaseTrigger
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _soundtrackManager = GetTree().Root.GetNode<SoundtrackManager>("SoundtrackManager");
        }

        protected override void OnEntered(Player player)
        {
            _soundtrackManager.PlayAmbient(_enterEventPath);
        }

        protected override void OnExited(Player player)
        {
            _soundtrackManager.PlayAmbient(_exitEventPath);
        }

        [Export] private string _enterEventPath;
        [Export] private string _exitEventPath;
        private SoundtrackManager _soundtrackManager;
    }
}
