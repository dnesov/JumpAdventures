using Godot;
using System;

public class LevelSelect : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public Action<short> LevelButtonPressed;

    public void CreateLevelButtons(Jump.Levels.Level[] levels)
    {
        var container = GetNode<Container>("%LevelButtons");

        short i = 0;
        foreach (var level in levels)
        {
            // var button = new LevelSelectButton(i)
            // {
            //     Text = $"Level {i + 1}"
            // };
            // button.OnPressed += ButtonPressed;
            // container.AddChild(button);

            var levelButton = _levelButtonScene.Instance<LevelButton>();
            levelButton.LevelId = i;
            levelButton.LevelName = level.Name;
            levelButton.UpdateElements();

            levelButton.OnPressed += ButtonPressed;

            container.AddChild(levelButton);

            i++;

        }
    }

    private void ButtonPressed(short levelId)
    {
        LevelButtonPressed.Invoke(levelId);
    }

    private void Close()
    {
        ClearButtons();
        Visible = false;
    }

    private void ClearButtons()
    {
        var container = GetNode<Container>("%LevelButtons");
        foreach (Control child in container.GetChildren())
        {
            child.QueueFree();
        }
    }

    [Export] private PackedScene _levelButtonScene;
}
