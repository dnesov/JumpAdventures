using System;
using Godot;

namespace Jump.Entities
{
    public class HealthHandler : Node
    {
        public HealthHandler(int maxHearts)
        {
            _maxHearts = maxHearts;
            _hearts = _maxHearts;
        }

        public int Hearts { get => _hearts; set => _hearts = value; }

        public int MaxHearts { get => _maxHearts; set => _maxHearts = value; }
        public bool Dead => _dead;

        public Action<int, DamageType> OnDamage;
        public Action<DamageType> OnAnyDamage;
        public Action<int> OnHeal;
        public Action OnDeath;
        public float TimeSinceDamage { get; set; }

        public override void _Process(float delta) => _timer += delta;

        public void Damage(int amount, float cooldown = 0.3f, DamageType damageType = DamageType.None)
        {
            _damageCooldown = cooldown;
            if (_timer < _damageCooldown) return;
            _timer = 0;

            if (amount >= _hearts || _hearts <= 0)
            {
                _hearts = 0;
                Die();
                return;
            }

            _hearts -= amount;

            OnDamage?.Invoke(amount, damageType);
            OnAnyDamage?.Invoke(damageType);
        }

        public void Heal(int amount)
        {
            _hearts += amount;
            if (_hearts > MaxHearts) _hearts = MaxHearts;

            OnHeal?.Invoke(amount);
        }

        public void Revive()
        {
            _hearts = _maxHearts;
            _dead = false;
        }

        public void Kill() => Damage(_hearts, 0);

        private void Die()
        {
            if (_dead) return;
            _dead = true;
            OnDeath?.Invoke();
        }

        private bool _dead;
        [Export] private int _maxHearts = 3;
        private int _hearts;
        private float _timer;
        private float _damageCooldown;
    }

    public enum DamageType
    {
        None,
        Spike
    }
}