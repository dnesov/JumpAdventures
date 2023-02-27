namespace GodotFmod
{
    public interface IStoppable
    {
        public void Stop(EventStopMode stopMode = EventStopMode.FadeOut);
    }
}