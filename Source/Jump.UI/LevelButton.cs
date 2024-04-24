using Godot;
using System;

public class LevelButton : TextureButton
{
    public short LevelId { get; set; }
    public string LevelName { get => _levelName; set => _levelName = value; }
    public int Attempts { get => _attempts; set => _attempts = value; }
    public int Orbs { get => _orbs; set => _orbs = value; }
    public int MaxOrbs { get => _maxOrbs; set => _maxOrbs = value; }
    public bool Completed { get => _completed; set => _completed = value; }

    public Action<short> OnPressed;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("pressed", this, nameof(OnButtonPressed));
    }

    private void OnButtonPressed()
    {
        OnPressed?.Invoke(LevelId);
    }

    public void UpdateElements()
    {
        GetNode<Label>("%Name").Text = LevelName;
        GetNode<Label>("%Attempts").Text = $"Attempts: {Attempts}";
        GetNode<Label>("%Orbs").Text = $"Orbs: {Orbs}/{MaxOrbs}";
    }

    private string _levelName;
    private int _attempts;
    private bool _completed;
    private int _orbs;
    private int _maxOrbs;

}
