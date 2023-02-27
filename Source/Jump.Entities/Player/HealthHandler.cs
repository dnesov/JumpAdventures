using System;
using Godot;

namespace Jump.Entities
{
    public class HealthHandler : Node
    {
        public int Health { get => _health; set => _health = value; }

        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public bool Dead => _dead;

        public Action<int> OnDamage;
        public Action OnAnyDamage;
        public Action<int> OnHeal;
        public Action OnDeath;

        public HealthHandler() => _health = _maxHealth;
        public override void _Process(float delta) => _timer += delta;

        public void Damage(int amount, float cooldown = 0.3f)
        {
            _damageCooldown = cooldown;
            if (_timer < _damageCooldown) return;
            _timer = 0;

            if (amount >= _health || _health <= 0)
            {
                _health = 0;
                Die();
                return;
            }

            _health -= amount;

            OnDamage?.Invoke(amount);
            OnAnyDamage?.Invoke();
        }

        public void Heal(int amount)
        {
            _health += amount;
            if (_health > MaxHealth) _health = MaxHealth;

            OnHeal?.Invoke(amount);
        }

        public void Revive()
        {
            _health = _maxHealth;
            _dead = false;
        }

        public void Kill() => Damage(_health, 0);

        private void Die()
        {
            if (_dead) return;
            _dead = true;
            OnDeath?.Invoke();
        }

        private bool _dead;
        [Export] private int _maxHealth = 3;
        private int _health;
        private float _timer;
        private float _damageCooldown;
    }
}