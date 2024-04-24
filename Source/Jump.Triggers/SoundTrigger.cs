using Godot;
using GodotFmod;
using Jump.Entities;
using Jump.Extensions;

namespace Jump.Triggers
{
    public class SoundTrigger : BaseTrigger
    {
        public override void _Ready()
        {
            var fmodRuntime = this.GetSingleton<FmodRuntime>();
            _eventInstance = fmodRuntime.GetEventInstance(_eventPath);
        }
        protected override void OnEntered(Player player)
        {
            if (_playOnce && _played) return;
            _played = true;
            _eventInstance.Start();
        }

        protected override void OnExited(Player player) { }

        private FmodEventInstance _eventInstance;
        private bool _played;
        [Export] private bool _playOnce;
        [Export] private string _eventPath;
    }
}