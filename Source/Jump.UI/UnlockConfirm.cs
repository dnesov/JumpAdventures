using Godot;
using Jump.Unlocks;
using System;

namespace Jump.UI;

public class UnlockConfirm : UIElement<UnlockableBase>
{
    public UnlockableBase Unlockable { get; private set; }

    public Action<UnlockableBase> OnConfirmed;
    public Action OnExited;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _yesButton = GetNode<JAButton>("%Yes");
        _yesButton.OnPressedAction += () =>
        {
            PlayHideAnimation();
            OnConfirmed?.Invoke(Unlockable);
        };
    }

    protected override void OnDisplay()
    {
        PlayDisplayAnimation();
        _yesButton.GrabFocus();
    }

    protected override void OnHide()
    {
        PlayHideAnimation();
        OnExited?.Invoke();
    }

    protected override void OnUpdateElements(UnlockableBase data)
    {
        Unlockable = data;

        var preview = GetNode<Control>("%Preview");
        var icon = preview.GetNode<TextureRect>("%Icon");
        var price = GetNode<Label>("%Price");

        icon.Texture = Unlockable.Icon;
        icon.Modulate = Unlockable.IconModulate;

        if (data.HasCondition<EssenceUnlockCondition>())
        {
            var condition = data.GetCondition<EssenceUnlockCondition>();
            price.Text = condition.EssenceRequired.ToString();
        }
    }

    private void PlayDisplayAnimation()
    {
        if (_tween != null && _tween.IsRunning())
        {
            _tween.Kill();
        }

        _tween = CreateTween();
        Visible = true;
        _tween.TweenProperty(this, "modulate:a", 1.0f, _tweenDuration);
    }

    private void PlayHideAnimation()
    {
        if (_tween != null && _tween.IsRunning())
        {
            _tween.Kill();
        }

        _tween = CreateTween();
        _tween.SetParallel();
        _tween.TweenProperty(this, "modulate:a", 0.0f, _tweenDuration);
        _tween.Chain().TweenCallback(this, nameof(SetInvisible));
    }

    private void SetInvisible()
    {
        Visible = false;
    }

    private SceneTreeTween _tween;

    private float _tweenDuration = 0.16f;
    private JAButton _yesButton;
}