using FMOD.Studio;

namespace GodotFmod
{
    public class FmodBus
    {
        internal FmodBus(Bus bus)
        {
            _bus = bus;
        }
        public bool Mute
        {
            get
            {
                return _mute;
            }
            set
            {
                _mute = value;
                SetMute(_mute);
            }
        }
        public bool Paused
        {
            get
            {
                return _paused;
            }
            set
            {
                _paused = value;
                SetPaused(_paused);
            }
        }
        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
                SetVolume(_volume);
            }
        }

        private void SetVolume(float volume)
        {
            _bus.setVolume(volume);
        }

        private float GetVolume()
        {
            float volume;
            _bus.getVolume(out volume);

            return volume;
        }

        private void SetMute(bool mute)
        {
            _bus.setMute(mute);
        }

        private bool GetMute()
        {
            bool mute;
            _bus.getMute(out mute);

            return mute;
        }

        private void SetPaused(bool pause)
        {
            _bus.setPaused(pause);
        }

        private bool _mute;
        private bool _paused;
        private float _volume;
        private Bus _bus;
    }
}
