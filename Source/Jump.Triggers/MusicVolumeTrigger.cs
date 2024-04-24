using Godot;
using Jump.Entities;
using Jump.Extensions;
using Jump.Utils;
using System;

namespace Jump.Triggers
{
    public class MusicVolumeTrigger : BaseTrigger
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _soundtrackManager = this.GetSingleton<SoundtrackManager>();
        }

        protected override void OnEntered(Player player)
        {
            _soundtrackManager.MusicVolume = 0.0f;
        }

        protected override void OnExited(Player player)
        {
            var game = this.GetSingleton<Game>();
            _soundtrackManager.MusicVolume = game.DataHandler.Data.Settings.AudioSettings.MusicVolume;
        }

        [Export] private string _enterEventPath;
        [Export] private string _exitEventPath;
        private SoundtrackManager _soundtrackManager;
    }
}
