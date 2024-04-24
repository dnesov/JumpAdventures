using Godot;

namespace Jump.UI;

public class LeaderboardEntryUI : Label
{
    public string Nick { get; set; }
    public float Time { get; set; }


    public override void _Ready()
    {
        base._Ready();
        Text = $"{Nick}: {string.Format(Constants.TimerFormat, Time)}";
    }
}
