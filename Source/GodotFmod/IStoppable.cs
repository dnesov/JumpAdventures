namespace GodotFmod
{
    public interface IStoppable
    {
        void Stop(EventStopMode stopMode = EventStopMode.FadeOut);
    }
}