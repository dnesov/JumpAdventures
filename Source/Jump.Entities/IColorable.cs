using Godot;

namespace Jump.Entities
{
    public interface IColorable
    {
        EntityType EntityType { get; }
        void Colorize(Color color);
    }
}