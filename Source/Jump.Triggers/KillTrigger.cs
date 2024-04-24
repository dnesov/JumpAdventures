using Jump.Entities;

namespace Jump.Triggers
{
    public class KillTrigger : BaseTrigger
    {
        protected override void OnEntered(Player player)
        {
            player.HealthHandler.Kill();
        }

        protected override void OnExited(Player player)
        {
            return;
        }
    }
}