using Godot;
using Jump.UI;
using Jump.UI.Menu;

public class AlphaNotice : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Visible = true;
        // var continueButton = GetNode<JAButton>("%Continue");
        // continueButton.GrabFocus();
    }

    private void Accept()
    {
        Visible = false;
        var mainSection = GetNode<MainSection>("%MainSection");
        mainSection.Focus();
    }
}
