using Godot;

namespace Jump.UI
{
    public class IconSocialButton : IconButton
    {
        public override void _Ready()
        {
            base._Ready();
            Connect("pressed", this, nameof(OnPressed));

        }

        private void OnPressed()
        {
            OS.ShellOpen(_link);
        }

        [Export] private string _link;
    }
}