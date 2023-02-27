using Godot;

public class FreeStreamPlayer : AudioStreamPlayer
{
    public override void _Ready()
    {
        Connect("finished", this, nameof(Finished));
    }

    private void Finished()
    {
        QueueFree();
    }
}