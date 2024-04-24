using Godot;

namespace Jump.Extensions
{
    public static class SceneTreeTimerExtensions
    {
        public static SignalAwaiter TimeOut(this SceneTreeTimer timer)
        {
            return timer.ToSignal(timer, "timeout");
        }
    }
}