using Godot;
using GodotFmod;
using Jump.Extensions;
using Jump.Utils;

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
            _fmodRuntime = this.GetSingleton<FmodRuntime>();
        }

        protected override async void Collected(Player player)
        {
            base.Collected(player);

            PlayPickupSound();
            PickupVisuals();
            player.Effects.Flash(new Color(1f, 1f, 1f, 0.3f), 4f);

            var progressHandler = this.GetSingleton<ProgressHandler>();
            var game = this.GetSingleton<Game>();
            var notificationManager = this.GetSingleton<NotificationManager>();

            progressHandler.CollectFragment();

            if (progressHandler.GlobalFragments == 1)
            {
                game.AchievementProvider.TrySetAchievement(AchievementIds.GAME_FRAGMENT_COLLECTED);
            }

            game.AchievementProvider.AddStat(StatIds.FRAGMENTS, 1);

            var notification = new Notification("UI_FRAGMENT_COLLECTED", string.Empty, 2.5f)
            {
                Icon = _notificationTexture,
                IconModulate = new Color(1f, 1f, 1f)
            };
            await notificationManager.AddNotification(notification);
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
            var particleSpawner = this.GetSingleton<ParticleSpawner>();
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
        [Export] private Texture _notificationTexture;
        private FmodRuntime _fmodRuntime;
    }
}
