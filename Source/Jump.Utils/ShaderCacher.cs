using Godot;
using Godot.Collections;

namespace Jump;

public class ShaderCacher : Node
{


    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        _logger.Info("Starting shader cache...");

        InstantiateNodes();

        for (int i = 0; i < _framesToIdle; i++)
        {
            await ToSignal(GetTree(), "idle_frame");
        }

        RemoveNodes();

        _logger.Info("Shader cache complete.");
    }

    private void InstantiateNodes()
    {
        foreach (var packedScene in _nodesToCache)
        {
            _logger.Info($"Caching {packedScene.ResourcePath}...");

            var node = packedScene.Instance<Node2D>();
            AddChild(node);

            if (node is Particles2D particles)
            {
                particles.Restart();
            }
        }
    }

    private void RemoveNodes()
    {
        foreach (Node node in GetChildren())
        {
            node.QueueFree();
        }
    }

    private Logger _logger = new(nameof(ShaderCacher));
    [Export] private int _framesToIdle = 10;
    [Export] private Array<PackedScene> _nodesToCache = new();
}
