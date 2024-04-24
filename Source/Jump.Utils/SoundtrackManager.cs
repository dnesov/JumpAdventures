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
                _intensity = Mathf.Clamp(value, 0.0f, 1.0f);
                _currentTrackEvent?.SetParameter("Intensity", _intensity);
            }
        }

        private float _end;
        public float End
        {
            get => _end; set
            {
                _end = value;
                _currentTrackEvent?.SetParameter("End", _end);
            }
        }

        private float _damaged;
        public float Damaged
        {
            get => _damaged; set
            {
                _damaged = value;
                _currentTrackEvent?.SetParameter("Damaged", _damaged);
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

        public string CurrentTrackPath => _currentTrackPath;

        public override void _Ready()
        {
            _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
        }

        public void PlayTrack(string path)
        {
            if (path == _currentTrackPath) return;
            _currentTrackEvent?.Stop();
            _currentTrackPath = path;
            _currentTrackEvent = _fmodRuntime.GetEventInstance(_currentTrackPath);
            _currentTrackEvent.Start();
            _currentTrackEvent.SetParameter("Volume", 1.0f);
        }

        public void PlayAmbience(string path)
        {
            if (path == _currentAmbiencePath) return;
            _currentAmbienceEvent?.Stop();
            _currentAmbiencePath = path;
            _currentAmbienceEvent = _fmodRuntime.GetEventInstance(_currentAmbiencePath);
            _currentAmbienceEvent.Start();
        }


        private string _currentTrackPath = "";
        private string _currentAmbiencePath = "";
        private FmodEventInstance _currentAmbienceEvent;
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
