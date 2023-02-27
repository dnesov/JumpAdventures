using Godot;

namespace Jump.Entities
{
    public class PlayerVfx : Node
    {
        public PlayerVfx(PackedScene hurtParticlesScene, Particles2D walkParticles, PackedScene deathParticlesScene, PackedScene winParticlesScene, Player player)
        {
            _hurtParticlesScene = hurtParticlesScene;
            _walkParticles = walkParticles;

            _deathParticlesScene = deathParticlesScene;
            _winParticlesScene = winParticlesScene;
            _player = player;

            SubscribeEvents();
        }

        // TODO: Use ParticleSpawner singleton instead.
        public void SpawnParticlesAt(Particles2D particles, Vector2 at)
        {
            particles.Modulate = _player.Sprite.Modulate;
            AddChild(particles);
            particles.GlobalPosition = at;
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
        }

        private void OnWin()
        {
            SpawnParticlesAt(_winParticlesScene.Instance<Particles2D>(), _player.Sprite.GlobalPosition);
        }

        private void DamageVfx() => SpawnParticlesAt(_hurtParticlesScene.Instance<Particles2D>(), _player.GlobalPosition);

        private void DeathVfx()
        {
            SpawnParticlesAt(_deathParticlesScene.Instance<Particles2D>(), _player.Sprite.GlobalPosition);
        }

        private Player _player;
        private PackedScene _hurtParticlesScene;
        private Particles2D _walkParticles;
        private PackedScene _deathParticlesScene, _winParticlesScene;
    }
}