using System;
using FMOD.Studio;

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

        public void Release()
        {
            _instance.release();
        }

        public void Dispose()
        {
            Release();
        }

        private EventInstance _instance;
    }

    public enum EventStopMode : int
    {
        FadeOut,
        Immediate
    }
}