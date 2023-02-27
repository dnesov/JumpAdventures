using Godot;
using GodotFmod;

public class UISounds : Node
{
    public override void _Ready()
    {
        _random = new System.Random();
        _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
    }
    public void Play(UISoundType soundType, bool randomizePitch = false, float volume = 0)
    {
        switch (soundType)
        {
            case UISoundType.ButtonFocus:
                _fmodRuntime.PlayOneShot("event:/UI/ButtonHover");
                break;
            case UISoundType.ButtonDown:
                // Play(_buttonDownSound, randomizePitch, volume);
                break;
            case UISoundType.ButtonPress:
                _fmodRuntime.PlayOneShot("event:/UI/ButtonPress");
                break;
        }
    }

    public void Play(AudioStream stream, bool randomizePitch = false, float volume = 0)
    {
        var streamPlayer = new FreeStreamPlayer()
        {
            Bus = "UI",
            Stream = stream,
            PitchScale = randomizePitch ? Mathf.Clamp((float)_random.NextDouble() * 1.1f, 0.9f, 1.1f) : 1,
            VolumeDb = volume
        };

        AddChild(streamPlayer);
        streamPlayer.Play();
    }

    [Export] private AudioStream _buttonFocusSound;
    [Export] private AudioStream _buttonDownSound;
    [Export] private AudioStream _buttonPressSound;

    private System.Random _random;
    private FmodRuntime _fmodRuntime;
}

public enum UISoundType
{
    ButtonFocus,
    ButtonDown,
    ButtonPress
}

