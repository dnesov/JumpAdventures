/// <summary>
/// Abstract class representing third-party activities, such as Discord rich presence.
/// </summary>
public abstract class Activity
{
    public abstract void Register();
    public abstract void Set(ActivityData data);
    public abstract void Process();
    public abstract void Shutdown();
}