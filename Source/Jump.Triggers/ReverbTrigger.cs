using Godot;
using GodotFmod;
using Jump.Entities;
using Jump.Extensions;
using System;

namespace Jump.Triggers
{
    public class ReverbTrigger : BaseTrigger
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _fmodRuntime = this.GetSingleton<FmodRuntime>();
        }

        protected override void OnEntered(Player player)
        {
            _fmodRuntime.PlayOneShot(_reverbSnapshotPath);
        }

        protected override void OnExited(Player player)
        {
            _fmodRuntime.PlayOneShot("snapshot:/No Reverb");
        }

        [Export] private string _reverbSnapshotPath;
        private FmodRuntime _fmodRuntime;
    }
}