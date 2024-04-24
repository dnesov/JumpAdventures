using Godot;

namespace Jump.Utils
{
    public class DebugTools : Node
    {
        public void TakeScreenshot()
        {
            var image = GetViewport().GetTexture().GetData();
            image.FlipY();
            image.SavePng($"user://{OS.GetUnixTime()}.png");
        }
    }
}