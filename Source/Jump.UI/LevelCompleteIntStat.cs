using Godot;
using GodotFmod;
using Jump.Extensions;

namespace Jump.UI;

public class LevelCompleteIntStat : UIElement
{
    public SceneTreeTween Tweener => _tween;
    public int TargetValue { get; set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _particles = GetNode<Particles2D>("%TickParticles");
        _valueLabel = GetNode<Label>("%StatValue");

        _fmodRuntime = this.GetSingleton<FmodRuntime>();

        Modulate = new Color(Modulate.r, Modulate.g, Modulate.b, 0.0f);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _tween?.Pause();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public void PauseTween()
    {
        _tween?.Pause();
    }

    protected override void OnDisplay()
    {
        _fmodRuntime.PlayOneShot(_displayEvent);

        Modulate = new Color(Modulate.r, Modulate.g, Modulate.b, 0.0f);
        RectPivotOffset = RectScale / 2;
        RectScale = Vector2.One * 1.2f;

        _tween = CreateTween().SetParallel();
        _tween.TweenProperty(this, "modulate:a", 1f, _tweenTime);
        _tween.SetTrans(Tween.TransitionType.Back);
        _tween.TweenProperty(this, "rect_scale", Vector2.One, _tweenTime);
        _tween.SetTrans(Tween.TransitionType.Linear);
        _tween.Chain();

        _tween.TweenProperty(_particles, "emitting", true, 0.0f);
        _tween.SetEase(Tween.EaseType.Out);

        // var labelAnimationTime = TargetValue < 5 ? _tweenTime : _tweenTime * 2f;

        if (TargetValue <= 1)
        {
            AnimateValueLabel(TargetValue);
            _tween.TweenProperty(_particles, "emitting", false, 0.0f);
            return;
        }

        _tween.TweenMethod(this, nameof(AnimateValueLabel), 0, TargetValue, _tweenTime);
        _tween.Chain();
        _tween.TweenProperty(_particles, "emitting", false, 0.0f);
    }

    protected override void OnHide()
    {

    }

    private void AnimateValueLabel(int value)
    {
        _valueLabel.Text = value.ToString();
        _fmodRuntime.PlayOneShot(_tickEvent);
    }

    [Export] private string _tickEvent = string.Empty;
    private Particles2D _particles;
    private Label _valueLabel;

    private SceneTreeTween _tween;
    private readonly string _displayEvent = "event:/UI/StatDisplay";
    private FmodRuntime _fmodRuntime;
    private float _tweenTime = 0.3f;
}