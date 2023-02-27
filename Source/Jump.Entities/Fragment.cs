using Godot;
using GodotFmod;
using Jump.Utils;
using System;

namespace Jump.Entities
{
    [Tool]
    public class Fragment : SerializedCollectible
    {
        public override void _Ready()
        {
            if (IsCollected())
                Modulate = new Color(1.0f, 1.0f, 1.0f, 0.35f);

            GetNodes();
        }

        private void GetNodes()
        {
            _fmodRuntime = GetTree().Root.GetNode<FmodRuntime>("FmodRuntime");
        }

        protected override void Collected(Player player)
        {
            base.Collected(player);

            PlayPickupSound();
            PickupVisuals();
            // player.ShowFragmentCollectGui();

            var game = GetTree().Root.GetNode<Game>("Game");
            game.CurrentWorldSaveData.CollectFragment();

            // QueueFree();
        }

        public override void _Process(float delta)
        {
            if (Engine.EditorHint) return;
            _timer += delta;
            Animate(delta);
        }

        private void PlayPickupSound()
        {
            _fmodRuntime.PlayOneShot(_pickupEvent);
        }

        private void PickupVisuals()
        {
            var animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
            animationPlayer.Play("pickup");

            var particles = _pickupParticlesScene.Instance<Particles2D>();
            var particleSpawner = GetTree().Root.GetNode<ParticleSpawner>(nameof(ParticleSpawner));
            particleSpawner.SpawnParticlesAt(particles, GlobalPosition);
        }

        private void Animate(float delta)
        {
            Position = Position.LinearInterpolate(new Vector2(Position.x, Position.y + Mathf.Sin(_timer * 2.0f) * 10), delta);
            Rotation = Mathf.LerpAngle(Rotation, Mathf.Sin(_timer * 2.0f) * Mathf.Deg2Rad(10f), delta * 2.0f);
        }

        private float _timer;
        [Export] private string _pickupEvent;
        [Export] private PackedScene _pickupParticlesScene;
        private FmodRuntime _fmodRuntime;
    }
}
