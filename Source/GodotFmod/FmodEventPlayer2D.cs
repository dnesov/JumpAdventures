using Godot;

namespace GodotFmod
{
    public class FmodEventPlayer2D : Node2D, IParametrizable, IStartable, IStoppable
    {
        [Export] public bool Autostart { get => _autostart; set => _autostart = value; }

        [Export]
        public float Volume
        {
            get => _volume; set
            {
                _volume = value;
                if (_eventInstance == null) return;
                _eventInstance.Volume = _volume;
            }
        }

        [Export] public float PanningFactor { get => _panningFactor; set => _panningFactor = value; }

        public override void _Ready()
        {
            _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
            _eventInstance = _fmodRuntime.GetEventInstance(_eventPath);

            if (!Autostart) return;
            _eventInstance.Start();
        }

        public override void _ExitTree()
        {
            _eventInstance.Stop();
            _eventInstance.Release();
        }

        public override void _Process(float delta)
        {
            _eventInstance.SetParameter("Pan", CalculatePanning() * _panningFactor);
            _eventInstance.SetParameter("Attenuation", CalculateAttenuation());
        }

        private float CalculatePanning()
        {
            return GlobalPosition.DirectionTo(_fmodRuntime.CurrentListener.GlobalPosition).x * -1;
        }

        private float CalculateAttenuation()
        {
            float attenuation = _maxDistance / GlobalPosition.DistanceTo(_fmodRuntime.CurrentListener.GlobalPosition);
            return Mathf.Clamp(attenuation, 0, 1);
        }

        public void SetParameter(string name, float value, bool ignoreSeekSpeed = false)
        {
            _eventInstance.SetParameter(name, value, ignoreSeekSpeed);
        }

        public void SetParameter(string name, string label, bool ignoreSeekSpeed = false)
        {
            _eventInstance.SetParameter(name, label, ignoreSeekSpeed);
        }

        public void Start()
        {
            _eventInstance.Start();
        }

        public void Stop(EventStopMode stopMode = EventStopMode.FadeOut)
        {
            _eventInstance.Stop(stopMode);
        }

        private float _volume = 1f;
        [Export] private float _maxDistance = 1000f;
        private float _panningFactor = 100f;
        [Export] private string _eventPath;
        private bool _autostart;
        private FmodRuntime _fmodRuntime;
        private FmodEventInstance _eventInstance;
    }
}