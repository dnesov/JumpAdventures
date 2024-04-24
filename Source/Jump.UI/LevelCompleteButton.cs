using System;
using Godot;

namespace Jump.UI;
[Tool]
public class LevelCompleteButton : JAButton
{
    [Export]
    public bool LeftAlign
    {
        get => _leftAlign; set
        {
            _leftAlign = value;
            if (_labelContainer != null)
            {
                _labelContainer.Alignment = _leftAlign ? BoxContainer.AlignMode.Begin : BoxContainer.AlignMode.End;
                _labelContainer.MoveChild(_actionIcon, _leftAlign ? 0 : 1);
            }
        }
    }

    [Export]
    public Texture Icon
    {
        get => _icon; set
        {
            _icon = value;
            if (_actionIcon == null) return;
            _actionIcon.Texture = _icon;
        }
    }

    [Export]
    public string Text
    {
        get => _text; set
        {
            _text = value;
            if (_label == null) return;
            _label.Text = _text;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _labelContainer = GetNode<HBoxContainer>("%LabelContainer");
        _actionIcon = GetNode<TextureRect>("%ActionIcon");
        _label = GetNode<Label>("%Label");
        _panelContainer = GetChild<PanelContainer>(0);

        _label.Text = Text;
        _actionIcon.Texture = _icon;

        _labelContainer.Alignment = _leftAlign ? BoxContainer.AlignMode.Begin : BoxContainer.AlignMode.End;
        _labelContainer.MoveChild(_actionIcon, _leftAlign ? 0 : 1);

        _tween = new Tween();
        AddChild(_tween);
    }

    protected override void OnFocusEntered()
    {
        PlayFocusAnimation();
        fmodRuntime.PlayOneShot(_buttonHoverEvent);
    }

    protected override void OnFocusExited()
    {
        PlayFocusExitAnimation();
    }

    protected override void OnPressed()
    {
        PlayButtonUpAnimation();
        FocusMode = FocusModeEnum.None;
        ReleaseFocus();
        fmodRuntime.PlayOneShot(_buttonPressEvent);
    }

    protected override void OnButtonDown()
    {
        PlayButtonDownAnimation();
    }

    protected override void OnButtonUp()
    {
        PlayButtonUpAnimation();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    private void PlayFocusAnimation()
    {
        _tween.StopAll();
        _tween.InterpolateProperty(_labelContainer, "modulate", new Color(1f, 1f, 1f), new Color(0f, 0f, 0f), tweenTime, default, Tween.EaseType.Out);
        _tween.InterpolateProperty(_panelContainer, "self_modulate", new Color(0f, 0f, 0f), new Color(1f, 1f, 1f), tweenTime, default, Tween.EaseType.Out);
        _tween.Start();
    }

    private void PlayFocusExitAnimation()
    {
        _tween.StopAll();
        _tween.InterpolateProperty(_labelContainer, "modulate", new Color(0f, 0f, 0f), new Color(1f, 1f, 1f), tweenTime, default, Tween.EaseType.Out);
        _tween.InterpolateProperty(_panelContainer, "self_modulate", new Color(1f, 1f, 1f), new Color(0f, 0f, 0f), tweenTime, default, Tween.EaseType.Out);
        _tween.Start();
    }

    private void PlayButtonDownAnimation()
    {
        RectPivotOffset = RectSize / 2;

        var initialScale = Vector2.One;
        var targetScale = Vector2.One * 0.8f;

        _tween.InterpolateProperty(this, "rect_scale", initialScale, targetScale, tweenTime, Tween.TransitionType.Quad, Tween.EaseType.Out);
        _tween.Start();
    }

    private void PlayButtonUpAnimation()
    {
        RectPivotOffset = RectSize / 2;

        var initialScale = Vector2.One;

        _tween.InterpolateProperty(this, "rect_scale", RectScale, initialScale, tweenTime * 2f, Tween.TransitionType.Back, Tween.EaseType.Out);
        _tween.Start();
    }

    private Tween _tween;

    private bool _leftAlign;
    private HBoxContainer _labelContainer;
    private TextureRect _actionIcon;
    private PanelContainer _panelContainer;
    private Label _label;

    private Texture _icon;
    private string _text = "";

    private string _buttonHoverEvent = "event:/UI/LCButtonHover";
    private string _buttonPressEvent = "event:/UI/LCButtonPress";
}
