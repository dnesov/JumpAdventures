using System;
using Godot;

namespace GodotFmod
{
    public class FmodRuntime : Node
    {
        public float CpuDspUsage => GetCpuDspUsage();
        public int MemoryUsage => GetMemoryUsage();
        public float Volume { get; }

        public Node2D CurrentListener { get; set; }

        public override void _Ready()
        {
            Initialize();
        }

        public override void _Process(float delta)
        {
            Update();
        }

        public override void _ExitTree()
        {
            Cleanup();
        }

        private void Initialize()
        {
            Print("Initializing...");
            var result = FMOD.Factory.System_Create(out _fmodSystem);
            _fmodSystem.setSoftwareFormat(48000, FMOD.SPEAKERMODE.STEREO, 0);
            if (result != FMOD.RESULT.OK)
            {
                throw new Exception($"Failed to initialize FMOD: {result}");
            }

            CreateStudioSystem();
        }

        private void Cleanup()
        {
            _fmodSystem.release();
            _fmodStudioSystem.release();
        }

        private void CreateStudioSystem()
        {
            FMOD.Studio.System.create(out _fmodStudioSystem);

            FMOD.Studio.INITFLAGS flag = FMOD.Studio.INITFLAGS.NORMAL;

            if (OS.HasFeature("editor"))
                flag = FMOD.Studio.INITFLAGS.LIVEUPDATE;

            FMOD.RESULT result;
            result = _fmodStudioSystem.initialize(1024, flag, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
            if (result != FMOD.RESULT.OK)
            {
                throw new Exception($"Failed to create FMOD Studio system: {result}");
            }

            LoadBanks();
        }

        public FmodBus GetBus(string path)
        {
            FMOD.Studio.Bus bus;
            CheckForError(_fmodStudioSystem.getBus(path, out bus), $"Failed to get bus \"{path}\"!");
            return new FmodBus(bus);
        }

        private void Update()
        {
            CheckForError(_fmodStudioSystem.update());
        }

        private void LoadBanks()
        {
            foreach (var bank in _banks)
            {
                LoadBank(bank.Path, out _fmodBank);
            }
        }

        public void LoadBank(string path, out FMOD.Studio.Bank bank)
        {
            Print($"Loading bank \"{path}\"...");

            var file = new File();
            file.Open(path, File.ModeFlags.Read);

            var buffer = file.GetBuffer((long)file.GetLen());
            bool error = CheckForError(_fmodStudioSystem.loadBankMemory(buffer, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out bank), "Failed to load bank!");

            if (!error)
                Print($"Bank \"{path}\" loaded!");

            file.Close();
        }

        public void PlayOneShot(string eventPath)
        {
            var ev = GetEventInstance(eventPath);
            ev.Start();
        }

        public FmodEventInstance GetEventInstance(string eventPath)
        {
            FMOD.Studio.EventDescription eventDescription;
            _fmodStudioSystem.getEvent(eventPath, out eventDescription);
            FMOD.Studio.EventInstance eventInstance;
            eventDescription.createInstance(out eventInstance);
            return new FmodEventInstance(eventInstance);
        }

        public void SetParameter(string name, float value, bool ignoreSeekSpeed = false)
        {
            _fmodStudioSystem.setParameterByName(name, value, ignoreSeekSpeed);
        }

        private void Print(object message)
        {
            GD.Print($"(FMOD) {message}");
        }

        private float GetCpuDspUsage()
        {
            FMOD.Studio.CPU_USAGE usage;
            FMOD.CPU_USAGE usageCore;
            _fmodStudioSystem.getCPUUsage(out usage, out usageCore);

            return usageCore.dsp;
        }

        private int GetMemoryUsage()
        {
            FMOD.Studio.MEMORY_USAGE usage;
            _fmodStudioSystem.getMemoryUsage(out usage);

            return usage.exclusive;
        }

        private float GetVolume()
        {
            // _fmodStudioSystem
            return 0.0f;
        }

        private bool CheckForError(FMOD.RESULT result, string errorMsg = "")
        {
            if (result == FMOD.RESULT.OK) return false;

            if (errorMsg == "")
                errorMsg = result.ToString();

            GD.PrintErr(errorMsg);
            return true;
        }

        private FMOD.System _fmodSystem = new FMOD.System();
        private FMOD.Studio.System _fmodStudioSystem;
        private FMOD.Studio.Bank _fmodBank;
        [Export(PropertyHint.ResourceType, "FmodBank")] private Godot.Collections.Array<FmodBank> _banks = new Godot.Collections.Array<FmodBank>();
    }
}