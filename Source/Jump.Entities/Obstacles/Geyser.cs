using Godot;
using GodotFmod;
using Jump.Entities;

[Tool]
public class Geyser : Area2D, IObstacle
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("item_rect_changed", this, nameof(RectChanged));
        GetNodes();
    }

    public override void _ExitTree()
    {
        _eventPlayer?.Stop(EventStopMode.Immediate);
    }

    private void GetNodes()
    {
        _eventPlayer = GetNode<FmodEventPlayer2D>("%FmodEventPlayer2D");
        _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
    }

    public void PlayerEntered(Player player)
    {
        _player = player;
        _playerInside = true;
        if (_state == GeyserState.Idle) return;
        DamagePlayer();
    }

    public void PlayerExited(Player player)
    {
        _playerInside = false;
    }

    public override void _Process(float delta)
    {
        var material = GetNode<Sprite>("%Sprite").Material as ShaderMaterial;
        material.SetShaderParam("zoom", GetViewportTransform().y.y);
    }

    private void RectChanged()
    {
        var material = GetNode<Sprite>("%Sprite").Material as ShaderMaterial;
        material.SetShaderParam("scale", Scale);
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
                    _eventPlayer?.SetParameter("Erupting", "False");
                }
                break;
            case GeyserState.Erupting:
                if (_playerInside)
                    DamagePlayer();

                if (_previousState == GeyserState.Idle)
                {
                    _animationPlayer?.Play(_eruptAnimName);
                    _eventPlayer?.Start();
                    _eventPlayer?.SetParameter("Erupting", "True");
                }

                break;
        }
    }

    private void DamagePlayer()
    {
        if (_player == null) return;
        _player.HealthHandler.Kill();
    }


    private float _intervalTimer;
    private float _durationTimer;
    [Export] private float _eruptionInterval = 5.0f;
    [Export] private float _eruptionDuration = 2.0f;
    private GeyserState _state;
    private GeyserState _previousState;
    private bool _playerInside;
    private Player _player;

    private AnimationPlayer _animationPlayer;
    private FmodEventPlayer2D _eventPlayer;

    private readonly string _idleToEruptAnimName = "idle_to_erupt";
    private readonly string _eruptToIdleAnimName = "erupt_to_idle";
    private readonly string _eruptAnimName = "erupt";
}

public enum GeyserState
{
    Idle,
    Erupting
}