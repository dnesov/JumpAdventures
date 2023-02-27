using Godot;
using Jump.Entities;

[Tool]
public class PolygonGround : Polygon2D, IColorable
{
    public EntityType EntityType => EntityType.Ground;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        collisionBody = GetNode<CollisionObject2D>("Body");
        collisionPolygon = GetNode<CollisionPolygon2D>("Body/CollisionPolygon2D");
    }

    public override void _Draw()
    {
        collisionPolygon.Polygon = Polygon;
    }

    public void Colorize(Color color)
    {
        Color = color;
    }

    protected CollisionObject2D collisionBody;
    protected CollisionPolygon2D collisionPolygon;
}
