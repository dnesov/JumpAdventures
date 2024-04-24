using Godot;

namespace Jump.Entities
{
    public class GravityOrb : OrbBase
    {
        protected override void OnPlayerEntered(Player player)
        {
            PlaySound(enterEventPath);
            PlayEnterAnimation();
        }

        protected override void OnInteract(Player player)
        {
            if (!player.GravityFlipped)
            {
                fmodRuntime.PlayOneShot(_flipEvent);
            }
            else
            {
                fmodRuntime.PlayOneShot(_unflipEvent);
            }

            player.FlipGravity();
            // PlaySound(interactEventPath);

            PlayInteractAnimation();
        }

        private string _flipEvent = "event:/GravityFlip";
        private string _unflipEvent = "event:/GravityUnflip";
    }
}