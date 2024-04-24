using System;
using FMOD.Studio;
using Godot;

namespace GodotFmod
{
    public class FmodEventInstance : IDisposable, IParametrizable, IStoppable, IStartable
    {
        public float Volume
        {
            get
            {
                float volume;
                _instance.getVolume(out volume);
                return volume;
            }
            set
            {
                _instance.setVolume(value);
            }
        }

        public FmodEventInstance(EventInstance nativeInstance)
        {
            _instance = nativeInstance;
        }

        public void Start()
        {
            _instance.start();
        }

        public void Stop(EventStopMode stopMode = EventStopMode.FadeOut)
        {
            _instance.stop((STOP_MODE)stopMode);
        }

        public void Pause()
        {
            _instance.setPaused(true);
        }

        public void Unpause()
        {
            _instance.setPaused(false);
        }

        public void SetParameter(string name, float value, bool ignoreSeekSpeed = false)
        {
            _instance.setParameterByName(name, value, ignoreSeekSpeed);
        }

        public void SetParameter(string name, string label, bool ignoreSeekSpeed = false)
        {
            _instance.setParameterByNameWithLabel(name, label, ignoreSeekSpeed);
        }

        public void Dispose()
        {
            _instance.release();
        }

        public void Set3DAttributes(Vector3 position, Vector3 velocity, Vector3 forward, Vector3 up)
        {
            FMOD.ATTRIBUTES_3D attributes = new()
            {
                position = FmodUtils.GodotVectorToFmod(position),
                velocity = FmodUtils.GodotVectorToFmod(velocity),
                forward = FmodUtils.GodotVectorToFmod(forward),
                up = FmodUtils.GodotVectorToFmod(up)
            };

            _instance.set3DAttributes(attributes);
        }

        private EventInstance _instance;
    }

    public enum EventStopMode : int
    {
        FadeOut,
        Immediate
    }
}