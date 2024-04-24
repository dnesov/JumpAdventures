using Godot;
using GodotFmod;
using Jump.Entities;
using Jump.Extensions;

[Tool]
public class Geyser : ResetableObstacle
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        Connect("item_rect_changed", this, nameof(RectChanged));
        GetNodes();
    }

    public override void _ExitTree()
    {
        _eruptSoundPlayer?.Stop(EventStopMode.Immediate);
    }

    public override void _Process(float delta)
    {
        var material = GetNode<Sprite>("%Sprite").Material as ShaderMaterial;
        material.SetShaderParam("zoom", GetViewportTransform().y.y);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_state == GeyserState.Idle)
            _intervalTimer += delta;

        if (_intervalTimer > _eruptionInterval - 1)
            _animationPlayer?.Play(_idleToEruptAnimName);

        if (_state == GeyserState.Erupting)
            _durationTimer += delta;

        if (_intervalTimer > _eruptionInterval)
        {
            SwitchState(GeyserState.Erupting);
            _intervalTimer = 0;
        }

        if (_durationTimer > _eruptionDuration)
        {
            SwitchState(GeyserState.Idle);
            _durationTimer = 0;
        }
    }

    protected override void OnPlayerEntered(Player player)
    {
        _player = player;
        if (_state == GeyserState.Idle) return;
        KillPlayer();
    }

    protected override void OnPlayerExited(Player player) { }

    protected override void OnRestart()
    {
        _durationTimer = 0f;
        _intervalTimer = 0f;
        SwitchState(GeyserState.Idle);
        _eruptSoundPlayer?.Stop(EventStopMode.Immediate);
    }

    private void GetNodes()
    {
        _eruptSoundPlayer = GetNode<FmodEventPlayer2D>("%EruptSound");
        _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
    }
    private void RectChanged()
    {
        var material = GetNode<Sprite>("%Sprite").Material as ShaderMaterial;
        material.SetShaderParam("scale", Scale);
    }

    private void SwitchState(GeyserState to)
    {
        _previousState = _state;
        _state = to;

        switch (to)
        {
            case GeyserState.Idle:
                if (_previousState == GeyserState.Erupting)
                {
                    _animationPlayer?.Play(_eruptToIdleAnimName);
                    _eruptSoundPlayer?.SetParameter("Erupting", "False");
                    break;
                }
                _animationPlayer?.Play("RESET");
                break;
            case GeyserState.Erupting:
                if (playerInside)
                {
                    KillPlayer();
                }

                if (_previousState == GeyserState.Idle)
                {
                    _animationPlayer?.Play(_eruptAnimName);
                    _eruptSoundPlayer?.Start();
                    _eruptSoundPlayer?.SetParameter("Erupting", "True");
                }

                break;
        }
    }

    private void KillPlayer()
    {
        if (_player == null) return;
        _closeCall = false;
        _player.HealthHandler.Kill();
    }

    private void OnCloseCallLeft(Node node)
    {
        if (node is Player player)
        {
            _player = player;
            if (!_player.IsOnFloor() && _state != GeyserState.Erupting) return;
            _player.OnLanded += GrantCloseCallAchievement;
        }
    }

    private void GrantCloseCallAchievement()
    {
        var game = this.GetSingleton<Game>();
        game.AchievementProvider.TrySetAchievement(AchievementIds.GAME_CLOSECALL);
        _player.OnLanded -= GrantCloseCallAchievement;
    }

    private float _intervalTimer;
    private float _durationTimer;
    [Export] private float _eruptionInterval = 5.0f;
    [Export] private float _eruptionDuration = 2.0f;
    private GeyserState _state;
    private GeyserState _previousState;
    private Player _player;

    private AnimationPlayer _animationPlayer;
    private FmodEventPlayer2D _eruptSoundPlayer;

    private bool _closeCall;

    private readonly string _idleToEruptAnimName = "idle_to_erupt";
    private readonly string _eruptToIdleAnimName = "erupt_to_idle";
    private readonly string _eruptAnimName = "erupt";
}

public enum GeyserState
{
    Idle,
    Erupting
}