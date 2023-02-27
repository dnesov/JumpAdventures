public abstract class Achievement
{
    public Achievement(AchievementHandler handler)
    {
        this.handler = handler;
    }

    private void SubscribeEvents()
    {

    }

    protected AchievementHandler handler;
}