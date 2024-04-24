using Godot;
using GodotFmod;
using Jump.Extensions;

namespace Jump.Entities
{
    public class OrbBase : Area2D, IObstacle
    {
        public override void _Ready()
        {
            GetNodes();
            animationPlayer.Play(idleAnimationName);
        }

        private void GetNodes()
        {
            fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
            animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        }

        public void PlayerEntered(Player player)
        {
            playerInside = true;
            _activated = false;

            _player = player;

            OnPlayerEntered(player);
        }

        public void PlayerExited(Player player)
        {
            playerInside = false;
            OnPlayerExited(player);
        }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed("move_jump"))
            {
                if (!playerInside || _activated) return;
                Interact();
            }

            if (Input.IsActionJustReleased("move_jump"))
            {
                StopInteract();
            }
        }

        internal void PlaySound(string eventPath)
        {
            fmodRuntime.PlayOneShot(eventPath);
        }

        protected void PlayEnterAnimation()
        {
            if (animationPlayer.CurrentAnimation == "interact") return;
            animationPlayer.Play(enterAnimationName);
        }

        protected void PlayExitAnimation()
        {
            if (animationPlayer.CurrentAnimation == "interact") return;
            animationPlayer.Play(exitAnimationName);
        }
        protected void PlayInteractAnimation() => animationPlayer.Play(interactAnimationName);

        protected virtual void OnPlayerEntered(Player player) { }
        protected virtual void OnPlayerExited(Player player) { }
        protected virtual void OnInteract(Player player) { }
        protected virtual void OnStopInteract(Player player) { }

        public void Enable()
        {
            Monitoring = true;
            Visible = true;
        }

        public void Disable()
        {
            Monitoring = false;
            Visible = false;
        }

        private void Interact()
        {
            OnInteract(_player);
            _activated = true;
            interacting = true;
        }

        private void StopInteract()
        {
            OnStopInteract(_player);
            interacting = false;
        }

        [Export] protected string interactEventPath;
        [Export] protected string enterEventPath;
        protected FmodRuntime fmodRuntime;
        protected AnimationPlayer animationPlayer;
        protected bool playerInside, interacting;
        private Player _player;
        private bool _activated;


        protected readonly string enterAnimationName = "on_enter";
        protected readonly string exitAnimationName = "on_exit";
        protected readonly string interactAnimationName = "interact";
        protected readonly string idleAnimationName = "idle";
    }
}