using Godot;
using Jump.Extensions;

public class Effects : Control
{
    public bool IsInsideWind { get; set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _effectAnimator = GetNode<AnimationPlayer>(_effectPlayerPath);
        _screenParticlesPlayer = GetNode<AnimationPlayer>(_screenParticlesPlayerPath);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (_windEffectCooldownActive)
        {
            _windCooldownTimer += delta;

            if (_windCooldownTimer >= _windEffectCooldown)
            {
                _windEffectCooldownActive = false;
                _windCooldownTimer = 0f;
                ReverseWindEffect();
            }
        }

        if (IsInsideWind)
        {
            _windCooldownTimer = 0f;
        }
    }

    public void Flash(Color color, float duration = 1f)
    {
        var game = this.GetSingleton<Game>();
        var settings = game.Data.Settings;

        if (!settings.AccessibilitySettings.FlashingEnabled)
        {
            return;
        }

        var flash = GetNode<ColorRect>("%Flash");

        flash.Visible = true;
        flash.Color = color;
        flash.Modulate = new Color(1f, 1f, 1f, 1f);

        if (_flashTween != null && _flashTween.IsRunning())
        {
            _flashTween.Stop();
        }

        _flashTween = CreateTween();
        _flashTween.TweenProperty(flash, "modulate:a", 0f, duration);
        _flashTween.TweenCallback(flash, "hide");
    }

    public void PlayDeathEffect() => _effectAnimator.Play(_deathAnimationName);
    public void ReverseDeathEffect() => _effectAnimator.PlayBackwards(_deathAnimationName);

    public void PlayIntroEffect(bool alreadyPlaying = false)
    {
        if (alreadyPlaying)
        {
            _effectAnimator.Play(_introAnimationName, default, 2.0f);
            return;
        }
        _effectAnimator.Play(_introAnimationName);
    }

    public void HideIntroEffect()
    {
        _effectAnimator.Play("intro_hidden");
    }

    public void PlayWindEffect()
    {
        if (_windEffectCooldownActive) return;
        _screenParticlesPlayer.Play(_windEnterAnimationName);
        _windEffectCooldownActive = true;
    }
    public async void ReverseWindEffect()
    {
        if (_reversingWindEffect || _windEffectCooldownActive) return;
        _reversingWindEffect = true;

        await this.TimeInSeconds(0.3f);

        _screenParticlesPlayer.PlayBackwards(_windEnterAnimationName);

        _reversingWindEffect = false;
    }

    public void PlayDamageEffect()
    {
        var damageVignette = GetNode<TextureRect>("%DamageVignette");

        var tween = CreateTween();
        // tween.SetParallel();
        tween.TweenProperty(damageVignette, "modulate:a", 0.65f, 0.05f);
        tween.TweenProperty(damageVignette, "modulate:a", 0.0f, 2.0f);
    }

    private SceneTreeTween _flashTween;
    private NodePath _effectPlayerPath = "EffectAnimator";
    private NodePath _screenParticlesPlayerPath = "ScreenParticlesAnimator";
    private AnimationPlayer _effectAnimator, _screenParticlesPlayer;

    private readonly string _deathAnimationName = "death";
    private readonly string _introAnimationName = "intro";
    private readonly string _windEnterAnimationName = "wind_enter";

    private float _windEffectCooldown = 0.5f;
    private bool _windEffectCooldownActive;
    private float _windCooldownTimer;
    private bool _reversingWindEffect;
}
