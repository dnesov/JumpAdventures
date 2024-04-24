namespace Jump.Entities
{
    public class JumpOrb : OrbBase
    {
        protected override void OnPlayerEntered(Player player)
        {
            PlaySound(enterEventPath);
            PlayEnterAnimation();
        }

        protected override void OnPlayerExited(Player player)
        {
            base.OnPlayerExited(player);
            PlayExitAnimation();
        }

        protected override void OnInteract(Player player)
        {
            player.Jump(shouldPlaySound: false);
            PlaySound(interactEventPath);
            PlayInteractAnimation();
        }
    }
}