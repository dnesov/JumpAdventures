using Godot;

namespace Jump.Entities
{
    public class DamageOrb : OrbBase
    {
        public override void _PhysicsProcess(float delta)
        {
            UpdateState(delta);
        }

        private void UpdateState(float delta)
        {
            if (_doDamage)
                _damageTimer += delta;

            if (!_doDamage)
                _timer += delta;

            if (_timer > _damageInterval)
            {
                _timer = 0;
                _doDamage = true;
                Modulate = new Color(1, 0, 0);
            }

            if (_doDamage && _damageTimer > _damageDuration)
            {
                _damageTimer = 0;
                _doDamage = false;
                Modulate = new Color(1, 1, 1);
            }
        }

        protected override void OnInteract(Player player)
        {
            base.OnInteract(player);
            player.Jump();
            PlaySound(interactEventPath);
            PlayInteractAnimation();

            if (!_doDamage) return;
            player.HealthHandler.Damage(1);
        }

        protected override void OnPlayerEntered(Player player)
        {
            PlaySound(enterEventPath);
            PlayEnterAnimation();
        }

        private bool _doDamage;
        private float _timer;
        private float _damageTimer;

        [Export] private float _damageInterval = 2.0f;
        [Export] private float _damageDuration = 0.65f;
    }
}