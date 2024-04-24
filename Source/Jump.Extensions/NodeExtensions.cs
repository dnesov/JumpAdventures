using Godot;

namespace Jump.Extensions
{
    public static class NodeExtensions
    {
        /// <summary>
        /// Shorthand for <c>GetTree().Root.GetNode()</c>. Gets a singleton Node that was instantiated from a script.
        /// </summary>
        /// <typeparam name="T">Type of the sigleton node.</typeparam>
        /// <param name="node"></param>
        /// <returns>The requested singleton with the type T.</returns>
        public static T GetSingleton<T>(this Node node) where T : Node
        {
            return node.GetTree().Root.GetNode<T>(typeof(T).Name);
        }

        public static T GetSingleton<T>(this Node node, string name) where T : Node
        {
            return node.GetTree().Root.GetNode<T>(name);
        }

        /// <summary>
        /// Shorthand for creating a <c>SceneTreeTimer</c> that awaits the specified amount of seconds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="timeSec">Time in seconds to wait for.</param>
        /// <param name="pauseModeProcess">Determines if the <c>SceneTreeTimer</c> should process when <c>SceneTree</c> is paused.</param>
        /// <returns>A <c>SignalAwaiter</c> that awaits until the created <c>SceneTreeTimer</c> is out.</returns>
        public static SignalAwaiter TimeInSeconds(this Node node, float timeSec, bool pauseModeProcess = true)
        {
            var timer = node.GetTree().CreateTimer(timeSec, pauseModeProcess);
            return timer.TimeOut();
        }
    }
}