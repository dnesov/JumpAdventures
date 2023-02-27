namespace Jump.Entities
{
    public class JumpOrb : OrbBase
    {
        protected override void OnPlayerEntered(Player player)
        {
            PlaySound(enterEventPath);
            PlayEnterAnimation();
        }

        protected override void OnInteract(Player player)
        {
            player.Jump();
            PlaySound(interactEventPath);
            PlayInteractAnimation();
        }
    }
}