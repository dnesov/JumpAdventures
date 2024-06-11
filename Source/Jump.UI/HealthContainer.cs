using System.Collections.Generic;
using Godot;
using Jump.Customize;
using Jump.Entities;
using Jump.Extensions;

namespace Jump.UI
{
    // TODO: Refactor as UIElement.
    public class HealthContainer : HBoxContainer
    {
        // Called when the node enters the scene tree for the first time.
        public override async void _Ready()
        {
            _player = GetOwner<Player>();
            await ToSignal(_player, "ready");

            _healthHandler = _player.HealthHandler;
            SubscribeEvents();

            var customizationHandler = this.GetSingleton<CustomizationHandler>();
            var prefs = customizationHandler.Preferences;
            var color = customizationHandler.GetColorById(prefs.ColorId).Lightened(0.6f);
            Modulate = color;
        }

        public override void _ExitTree()
        {
            UnsubscribeEvents();
        }

        public void SubscribeEvents()
        {
            _player.HealthHandler.OnDamage += UpdateHealthSprites;
            _player.HealthHandler.OnDeath += UpdateOnDeath;
            _player.OnAnyRespawn += OnAnyRespawn;
        }

        private void UnsubscribeEvents()
        {
            _player.HealthHandler.OnDamage -= UpdateHealthSprites;
            _player.HealthHandler.OnDeath -= UpdateOnDeath;
            _player.OnAnyRespawn -= OnAnyRespawn;
        }

        private void OnAnyRespawn()
        {
            Refill(_healthHandler.Hearts);
        }

        private HealthElement CreateNode()
        {
            var heart = _heartScene.Instance() as HealthElement;
            AddChild(heart);

            return heart;
        }

        public void Refill(int currentHealth)
        {
            _elements.Clear();

            if (GetChildren().Count != 0)
            {
                foreach (HealthElement child in GetChildren())
                {
                    child.Refill();
                    _elements.Push(child);
                }
                return;
            }
            for (int i = 0; i < _maxHearts; i++)
            {
                var heart = CreateNode();
                _elements.Push(heart);

                if (i < currentHealth)
                {
                    heart.MakeFull();
                }
                else
                {
                    heart.MakeEmpty();
                }
            }
        }

        public void PopulateHealthSprites(int currentHealth, int maxHealth)
        {
            _maxHearts = maxHealth;
            Refill(currentHealth);
        }
        public void UpdateHealthSprites(int damageAmount, DamageType damageType)
        {
            for (int i = 0; i < damageAmount; i++)
            {
                if (damageAmount > _maxHearts) return;
                var heart = _elements.Pop();
                heart.Deduct();
            }
        }

        public void UpdateOnDeath()
        {
            foreach (var heart in _elements)
            {
                heart.Deduct();
            }
            _elements.Clear();
        }

        private Player _player;
        [Export] private PackedScene _heartScene;
        private int _maxHearts;
        private HealthHandler _healthHandler;
        private Stack<HealthElement> _elements = new Stack<HealthElement>();
    }
}