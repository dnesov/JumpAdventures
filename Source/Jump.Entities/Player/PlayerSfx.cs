using Godot;
using GodotFmod;

namespace Jump.Entities
{
    /// <summary>
    /// Class responsible for playing Player entity SFX.
    /// </summary>
    public class PlayerSfx : Node
    {
        public PlayerSfx(Player player)
        {
            _player = player;
        }
        public override void _Ready()
        {
            _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
            _slideSound = _fmodRuntime.GetEventInstance(_slideEvent);

            // TODO: fix slide sound not playing for whatever reason.
            StartSlideSound();
            SubscribeEvents();
        }

        public override void _PhysicsProcess(float delta)
        {
            Update(delta);
            ModulateSlideSound(Mathf.Abs(_player.Velocity.x), _player.MaxSpeed, _player.GetFloorAngle(), _player.IsOnFloor());
        }

        public override void _ExitTree()
        {
            _slideSound.Stop();
            UnsubscribeEvents();
        }

        public void ModulateSlideSound(float speed, float maxSpeed, float angle, bool onFloor)
        {
            var speedParam = onFloor ? Mathf.Clamp(speed / maxSpeed, 0f, 1f) : 0f;
            var angleParam = Mathf.Clamp(angle / Mathf.Deg2Rad(45), 0f, 1f);

            _slideSound.SetParameter("Speed", speedParam);
            _slideSound.SetParameter("Angle", angleParam);
        }

        public void Update(float delta)
        {
            _essenceCombo -= 0.1f * delta;
            _essenceCombo = Mathf.Clamp(_essenceCombo, 0, 1);
        }

        public void StartSlideSound() => _slideSound.Start();
        public void StopSlideSound() => _slideSound.Stop();
        public void PlayJumpSound() => _fmodRuntime.PlayOneShot("event:/Player/Jump");
        public void PlayLandSound() => _fmodRuntime.PlayOneShot("event:/Player/Land");
        public void PlayHurtSound() => _fmodRuntime.PlayOneShot(_hurtEvent);
        public void PlayDeathSound() => _fmodRuntime.PlayOneShot(_deathEvent);
        public void PlayRestartSound() => _fmodRuntime.PlayOneShot("event:/Restart");
        public void PlayAdheasedSound() => _fmodRuntime.PlayOneShot("event:/Player/Glue");

        public void PlayEssenceSound(float combo = 0.0f)
        {
            var ev = _fmodRuntime.GetEventInstance("event:/Collect");
            _essenceCombo += combo;
            ev.Start();
            ev.SetParameter("Combo", _essenceCombo);
        }

        private void SubscribeEvents()
        {
            _player.HealthHandler.OnAnyDamage += PlayHurtSound;
            _player.HealthHandler.OnDeath += OnDeath;
            _player.OnAnyRespawn += OnRespawn;
            _player.OnWin += OnWin;
            _player.OnAnyJump += PlayJumpSound;
        }
        private void UnsubscribeEvents()
        {
            _player.HealthHandler.OnAnyDamage -= PlayHurtSound;
        }

        private void OnRespawn()
        {
            StartSlideSound();
            PlayRestartSound();
        }
        private void OnDeath()
        {
            PlayDeathSound();
            StopSlideSound();
        }

        private void OnWin() => StopSlideSound();

        private Player _player;
        private FmodRuntime _fmodRuntime;
        private readonly string _deathEvent = "event:/Player/Death";
        private readonly string _hurtEvent = "event:/Player/Hurt";
        private readonly string _slideEvent = "event:/Player/Slide";
        private FmodEventInstance _slideSound;

        private float _essenceCombo;
    }
}