using Godot;

namespace Jump.Entities
{
    public class PlayerVfx : Node
    {
        public override async void _Ready()
        {
            base._Ready();
            _player = GetOwner<Player>();
            await ToSignal(_player, "ready");
            SubscribeEvents();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        // TODO: Use ParticleSpawner singleton instead.
        public void SpawnParticlesAt(Particles2D particles, Vector2 at, bool inverted = false)
        {
            particles.Modulate = _player.Sprite.Modulate;
            AddChild(particles);
            particles.GlobalPosition = at;

            if (inverted)
            {
                particles.GlobalRotationDegrees = 180;
            }

            particles.Restart();
        }

        public void ChangeSpriteColor(Color color)
        {
            _player.Sprite.SelfModulate = color;
        }

        private void SubscribeEvents()
        {
            _player.OnWin += OnWin;
            _player.HealthHandler.OnAnyDamage += DamageVfx;
            _player.HealthHandler.OnDeath += DeathVfx;
            _player.OnLanded += SpawnLandParticles;
            _player.OnAnyJump += SpawnJumpParticles;
        }

        private void UnsubscribeEvents()
        {
            _player.OnWin -= OnWin;
            _player.HealthHandler.OnAnyDamage -= DamageVfx;
            _player.HealthHandler.OnDeath -= DeathVfx;
            _player.OnLanded -= SpawnLandParticles;
            _player.OnAnyJump -= SpawnJumpParticles;
        }

        private Particles2D Instantiate(PackedScene particleScene)
        {
            return particleScene.Instance<Particles2D>();
        }

        private void OnWin()
        {
            SpawnParticlesAt(_winParticlesScene.Instance<Particles2D>(), _player.Sprite.GlobalPosition);
        }

        private void DamageVfx(DamageType damageType)
        {
            SpawnParticlesAt(Instantiate(_hurtParticlesScene), _player.GlobalPosition);
        }

        private void DeathVfx()
        {
            SpawnParticlesAt(Instantiate(_deathParticlesScene), _player.Sprite.GlobalPosition);
        }

        private void SpawnJumpParticles()
        {
            SpawnParticlesAt(Instantiate(_jumpParticlesScene), _player.GlobalPosition, _player.GravityFlipped);
        }

        private void SpawnLandParticles()
        {
            var pos = _player.GlobalPosition;
            pos.y += 32;
            SpawnParticlesAt(Instantiate(_landParticlesScene), pos, _player.GravityFlipped);
        }


        private Player _player;
        [Export] private PackedScene _hurtParticlesScene;
        [Export] private PackedScene _deathParticlesScene;
        [Export] private PackedScene _winParticlesScene;
        [Export] private PackedScene _jumpParticlesScene;
        [Export] private PackedScene _landParticlesScene;
    }
}