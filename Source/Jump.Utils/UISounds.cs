using Godot;
using GodotFmod;

public class UISounds : Node
{
    public override void _Ready()
    {
        _random = new System.Random();
        _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
    }
    public void Play(UISoundType soundType)
    {
        switch (soundType)
        {
            case UISoundType.ButtonFocus:
                _fmodRuntime.PlayOneShot(_buttonHoverEvent);
                break;
            case UISoundType.ButtonDown:
                // Play(_buttonDownSound, randomizePitch, volume);
                break;
            case UISoundType.ButtonPress:
                _fmodRuntime.PlayOneShot("event:/UI/ButtonPress");
                break;
        }
    }

    [Export] private string _buttonHoverEvent = "event:/UI/ButtonHover";
    [Export] private string _buttonDownEvent = string.Empty;
    [Export] private string _buttonPressEvent = "event:/UI/ButtonPress";

    private System.Random _random;
    private FmodRuntime _fmodRuntime;
}

public enum UISoundType
{
    ButtonFocus,
    ButtonDown,
    ButtonPress
}

