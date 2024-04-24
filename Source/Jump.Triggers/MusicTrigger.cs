using Godot;
using Jump.Entities;
using Jump.Extensions;
using Jump.Utils;

namespace Jump.Triggers
{
    public class MusicTrigger : BaseTrigger
    {
        public override void _Ready()
        {
            _soundtrackManager = this.GetSingleton<SoundtrackManager>();
            _trackBeforeEntering = _soundtrackManager.CurrentTrackPath;
        }
        protected override void OnEntered(Player player)
        {
            _soundtrackManager.PlayTrack(_trackPath);
        }

        protected override void OnExited(Player player)
        {
            _soundtrackManager.PlayTrack(_trackBeforeEntering);
        }

        private string _trackBeforeEntering;
        [Export] private string _trackPath = string.Empty;
        private SoundtrackManager _soundtrackManager;
    }
}