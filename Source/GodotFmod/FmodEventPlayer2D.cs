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
            _eventInstance.Dispose();
        }

        public override void _Process(float delta)
        {
            var pos = new Vector3()
            {
                x = GlobalPosition.x,
                y = GlobalPosition.y,
                z = 0.0f
            };
            _eventInstance.Set3DAttributes(pos, Vector3.Zero, new Vector3(0f, 0f, -1f), Vector3.Down);
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
        [Export] private string _eventPath;
        private bool _autostart;
        private FmodRuntime _fmodRuntime;
        private FmodEventInstance _eventInstance;
    }
}