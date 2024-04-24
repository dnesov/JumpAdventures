using Godot;
using System;

[Tool]
public class WorldCard : PanelContainer
{

    [Export]
    public int WorldCount
    {
        get => _worldCount; set
        {
            _worldCount = value;
            // UpdateElements();
        }
    }

    [Export]
    public string WorldName
    {
        get => _worldName; set
        {
            _worldName = value;
            // UpdateElements();
        }
    }

    [Export]
    public short LastLevel
    {
        get => _lastLevel; set
        {
            _lastLevel = value;
            // UpdateElements();
        }
    }

    [Export]
    public short MaxLevels
    {
        get => _maxLevels; set
        {
            _maxLevels = value;
            // UpdateElements();
        }
    }

    [Export]
    public Texture WorldPreview
    {
        get => _worldPreview; set
        {
            _worldPreview = value;
            // UpdateElements();
        }
    }

    [Export]
    public Color WorldPreviewModulate
    {
        get => _worldPreviewModulate; set
        {
            _worldPreviewModulate = value;
            // UpdateElements();
        }
    }

    public bool WorldPlayable { get => _worldPlayable; set => _worldPlayable = value; }

    public short WorldId;

    private TextureButton _button;

    public Action<int> OnPressed;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _button = GetNode<TextureButton>(_buttonPath);
        _button.Connect("focus_entered", this, nameof(FocusEntered));
        _button.Connect("mouse_entered", this, nameof(MouseEntered));
        _button.Connect("focus_exited", this, nameof(FocusExited));
        _button.Connect("pressed", this, nameof(OnButtonPressed));

        if (!_worldPlayable)
        {
            GetNode<MarginContainer>("MarginContainer").Modulate = new Color(1.0f, 1.0f, 1.0f, 0.4f);
            GetNode<Label>("ComingSoonLabel").Visible = true;
            _button.Disabled = true;
        }
    }

    private void FocusEntered()
    {
        if (_button.Disabled) return;
        var tween = CreateTween().SetParallel();
        tween.SetTrans(Tween.TransitionType.Back);
        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "rect_scale", Vector2.One * 1.1f, 0.3f);
    }

    private void FocusExited()
    {
        var tween = CreateTween().SetParallel();
        tween.SetTrans(Tween.TransitionType.Back);
        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "rect_scale", Vector2.One, 0.3f);
    }

    private void MouseEntered()
    {
        if (!_button.Disabled)
        {
            _button.GrabFocus();
        }
    }

    public void SetFocus()
    {
        _button.GrabFocus();
    }


    private void OnButtonPressed()
    {
        if (!_worldPlayable) return;
        OnPressed.Invoke(_worldCount - 1);

        var tween = CreateTween().SetParallel();
        tween.TweenProperty(this, "rect_scale", new Vector2(0.95f, 0.95f), 0.2f / 4f);
        tween.Chain().TweenProperty(this, "rect_scale", new Vector2(1f, 1f), 0.2f / 2f);
    }

    public void UpdateElements()
    {
        GetNode<Label>(_worldCountLabel).Text = $"World {_worldCount}";
        GetNode<Label>(_worldNameLabel).Text = _worldName;
        GetNode<Label>(_worldProgressLabel).Text = $"{_lastLevel}/{_maxLevels}";
        GetNode<TextureRect>(_worldPreviewPath).Texture = _worldPreview;
        GetNode<TextureRect>(_worldPreviewPath).SelfModulate = _worldPreviewModulate;
    }

    private int _worldCount;
    private string _worldName;
    private bool _worldPlayable;
    private short _lastLevel;
    private short _maxLevels;
    private Texture _worldPreview;
    private Color _worldPreviewModulate = new Color(1, 1, 1, 1);

    [Export] private NodePath _worldCountLabel;
    [Export] private NodePath _worldNameLabel;
    [Export] private NodePath _worldProgressLabel;
    [Export] private NodePath _worldPreviewPath;
    [Export] private NodePath _buttonPath;
}
