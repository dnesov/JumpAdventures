using Godot;
using Jump.Extensions;

namespace Jump.UI;

public class WorldBackground : TextureRect
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        var game = this.GetSingleton<Game>();

        await ToSignal(game, "ready");

        var worldId = game.Data.LastWorldId;
        var path = Constants.WorldIds.ImagePaths["ja_world1_seaside"];

        if (Constants.WorldIds.ImagePaths.ContainsKey(worldId))
        {
            path = Constants.WorldIds.ImagePaths[worldId];
        }


        var image = GD.Load<StreamTexture>(path);

        Texture = image;
    }
}
