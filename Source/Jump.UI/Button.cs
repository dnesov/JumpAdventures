using Godot;

namespace Jump.UI
{
    [Tool]
    public class Button : TextureButton
    {
        [Export]
        public Texture Icon
        {
            get => _icon;
            set
            {
                _icon = value;

                GetNodes();
                UpdateIcon(_icon);
            }
        }
        [Export]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                GetNodes();
                UpdateLabel(_text);
            }
        }
        public override void _EnterTree()
        {
            GetNodes();
        }
        public override void _Ready()
        {
            ConnectSignals();
            GetNodes();
        }

        private void ConnectSignals()
        {
            Connect("focus_entered", this, nameof(FocusEntered));
            Connect("mouse_entered", this, nameof(MouseEntered));
            Connect("focus_exited", this, nameof(FocusExited));

            Connect("button_down", this, nameof(OnDown));
            Connect("pressed", this, nameof(OnPressed));
        }

        private void FocusEntered()
        {
            _animPlayer.Play("focus");
            PlayFocusSound();
        }

        private void MouseEntered()
        {
            if (!Disabled)
            {
                GrabFocus();
            }
        }

        private void MouseExited()
        {

        }

        private void FocusExited()
        {
            _animPlayer.PlayBackwards("focus");
        }

        private void OnDown()
        {
            _animPlayer.Play("press");
            GetTree().Root.GetNode<UISounds>("UiSounds").Play(UISoundType.ButtonDown, default, -5f);
        }


        private void OnPressed()
        {
            _animPlayer.PlayBackwards("press");
            PlayPressSound();
        }

        private void PlayFocusSound()
        {
            GetTree().Root.GetNode<UISounds>("UiSounds").Play(UISoundType.ButtonFocus, true, -15f);
        }

        private void PlayPressSound()
        {
            GetTree().Root.GetNode<UISounds>("UiSounds").Play(UISoundType.ButtonPress, true, -15f);
        }

        private void GetNodes()
        {
            _iconNode = GetNode<TextureRect>("VBoxContainer/HBoxContainer/Icon");
            _label = GetNode<Label>("VBoxContainer/HBoxContainer/Label");
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }

        private void UpdateLabel(string text)
        {
            _label.Text = text;
        }

        private void UpdateIcon(Texture icon)
        {
            _iconNode.Texture = icon;
        }

        private Texture _icon;
        private string _text;

        private TextureRect _iconNode;
        private Label _label;
        private AnimationPlayer _animPlayer;
    }
}