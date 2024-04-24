public class Ability
{
    public virtual void Process() { }
    public virtual void IdleProcess() { }
    public virtual void OnUse() { }
    public virtual void OnStopUse() {}
}