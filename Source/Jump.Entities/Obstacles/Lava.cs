using System;
using Godot;
using GodotFmod;
using Jump.Entities;

[Tool]
public class Lava : Area2D, IObstacle
{
    [Export]
    public float WavePeriod
    {
        get => _wavePeriod; set
        {
            _wavePeriod = value;
            UpdateUniform("wave_period", _wavePeriod);
        }
    }

    [Export]
    public float WaveSpeed
    {
        get => _waveSpeed; set
        {
            _waveSpeed = value;
            UpdateUniform("wave_speed", _waveSpeed);
        }
    }

    [Export]
    public float WaveAmplitude
    {
        get => _waveAmplitude; set
        {
            _waveAmplitude = value;
            UpdateUniform("wave_amplitude", _waveAmplitude);
        }
    }

    [Export]
    public Color ColorA
    {
        get => _colorA; set
        {
            _colorA = value;
            UpdateUniform("color_a", _colorA);
        }
    }

    [Export]
    public Color ColorB
    {
        get => _colorB; set
        {
            _colorB = value;
            UpdateUniform("color_b", _colorB);
        }
    }

    public void PlayerEntered(Player player)
    {
        player.HealthHandler.Kill();
        _fmodRuntime.PlayOneShot("event:/Player/WaterDeath");
    }

    public void PlayerExited(Player player) { }


    private void UpdateUniform(string name, object value)
    {
        if (IsInsideTree())
        {
            var sprite = GetNode<Sprite>("Sprite");
            var material = sprite.Material as ShaderMaterial;

            material.SetShaderParam(name, value);
        }
    }

    private void RectChanged()
    {
        UpdateUniform("scale", Scale);
    }

    public override void _Ready()
    {
        _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
    }

    public void Enable()
    {
        throw new NotImplementedException();
    }

    public void Disable()
    {
        throw new NotImplementedException();
    }

    private float _wavePeriod;
    private float _waveSpeed;
    private float _waveAmplitude;
    private Color _colorA;
    private Color _colorB;

    private FmodRuntime _fmodRuntime;
}
