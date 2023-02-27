using Godot;
using GodotFmod;

namespace Jump.Utils
{
    [Tool]
    public class SoundtrackManager : Node
    {
        private float _intensity;
        public float Intensity
        {
            get => _intensity; set
            {
                _intensity = value;
                _currentTrackEvent?.SetParameter("Intensity", _intensity);
            }
        }

        public TrackState CurrentTrackState
        {
            get => _currentTrackState; set
            {
                _currentTrackState = value;
                _currentTrackEvent?.SetParameter("State", _currentTrackState.ToString());
            }
        }

        public float MusicVolume
        {
            get => _currentTrackEvent == null ? 0f : _currentTrackEvent.Volume;
            set
            {
                if (_currentTrackEvent == null) return;
                _currentTrackEvent.SetParameter("Volume", value);
            }
        }

        public override void _Ready()
        {
            _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
        }

        public override void _PhysicsProcess(float delta)
        {
            // Intensity -= delta / 10;
            // Intensity = Mathf.Clamp(Intensity, 0, 1);
        }

        public void PlayTrack(string path, bool fadeIn = true)
        {
            if (path == _currentTrackPath) return;
            _currentTrackPath = path;
            _currentTrackEvent = _fmodRuntime.GetEventInstance(_currentTrackPath);
            _currentTrackEvent.Start();
            if (!fadeIn) return;
            _currentTrackEvent.SetParameter("Volume", 1.0f);
        }

        public void PlayAmbient(string path)
        {
            if (path == _currentAmbientPath) return;
            _currentAmbientEvent?.Stop();
            _currentAmbientPath = path;
            _currentAmbientEvent = _fmodRuntime.GetEventInstance(_currentAmbientPath);
            _currentAmbientEvent.Start();
        }


        private string _currentTrackPath = "";
        private string _currentAmbientPath = "";
        private FmodEventInstance _currentAmbientEvent;
        private FmodEventInstance _currentTrackEvent;
        private TrackState _currentTrackState = TrackState.Menu;
        private FmodRuntime _fmodRuntime;
    }

    public enum TrackState
    {
        Menu,
        InGame,
        InGameCinematic
    }
}
