using Godot;
using System;

public class Effects : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _effectAnimator = GetNode<AnimationPlayer>(_animationPlayerPath);
    }

    public void PlayDeathEffect() => _effectAnimator.Play(_deathAnimationName);
    public void ReverseDeathEffect()
    {
        _effectAnimator.PlayBackwards(_deathAnimationName);
    }

    private NodePath _animationPlayerPath = "EffectAnimator";
    private AnimationPlayer _effectAnimator;

    private readonly string _deathAnimationName = "death";
}
