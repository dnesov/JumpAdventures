using Godot;
using Godot.Collections;
using System;

public class LevelSelectMenu : MarginContainer
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateWorldCards();
    }

    private void CreateWorldCards()
    {
        var game = GetTree().Root.GetNode<Game>("Game");
        var worldpacks = game.GetWorldpacksOrdered("res://Levels/");

        int i = 0;
        foreach (var world in worldpacks)
        {
            if (world.Hidden) continue;
            var worldCard = _worldCardScene.Instance<WorldCard>();
            worldCard.WorldPlayable = world.Playable;
            GetNode<HBoxContainer>(_worldCardContainerPath).AddChild(worldCard);

            worldCard.WorldCount = i + 1;
            worldCard.WorldName = world.Name;
            worldCard.WorldPreviewModulate = new Color(world.PreviewImageModulate);
            worldCard.MaxLevels = (short)world.Levels.Length;
            worldCard.OnPressed += WorldCardPressed;
            worldCard.UpdateElements();

            if (i == 0)
            {
                worldCard.SetFocus();
            }

            i++;
        }
    }

    private void WorldCardPressed(int worldId)
    {
        var game = GetTree().Root.GetNode<Game>("Game");
        var worldpacks = game.GetWorldpacksOrdered("res://Levels/");
        _selectedWorld = worldpacks[worldId];

        game.LoadWorld(_selectedWorld);

        var levelSelect = GetNode<LevelSelect>(_levelSelectPath);
        levelSelect.Visible = true;
        levelSelect.CreateLevelButtons(_selectedWorld.Levels);

        levelSelect.LevelButtonPressed += LevelSelectButtonPressed;
    }

    private void LevelSelectButtonPressed(short levelId)
    {
        var game = GetTree().Root.GetNode<Game>("Game");
        var level = _selectedWorld.Levels[levelId];
        game.LoadLevel(level, true);
    }
    public void BackPressed()
    {
        var game = GetTree().Root.GetNode<Game>("Game");
        game.ReturnToMenu(true);
    }


    [Export] private PackedScene _worldCardScene;
    [Export] private NodePath _worldCardContainerPath;
    [Export] private NodePath _levelSelectPath;


    private Jump.Levels.World _selectedWorld;
}
