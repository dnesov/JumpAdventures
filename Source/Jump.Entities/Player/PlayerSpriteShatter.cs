using Godot;
using System;
using System.Collections.Generic;

namespace Jump.Entities
{
    // Thanks to:
    // https://www.reddit.com/r/godot/comments/nimkqg/how_to_break_a_2d_sprite_in_a_cool_and_easy_way/
    public class PlayerSpriteShatter : Node2D
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _playerSprite = GetParent<PlayerSprite>();
            _triangles = new List<Triangle>();
            _shards = new List<RigidBody2D>();
            // Break();
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (@event.IsActionPressed("debug")) Break();
        }

        public override void _Draw()
        {
            base._Draw();
            foreach (var t in _triangles)
            {
                DrawLine(t.P1, t.P2, new Color(1f, 0f, 0f));
                DrawLine(t.P1, t.P3, new Color(1f, 0f, 0f));
                DrawLine(t.P3, t.P1, new Color(1f, 0f, 0f));
            }
        }

        private void Break()
        {
            var points = CreatePoints();
            _triangles = CreateTriangles(points);
            ConstructShards(_triangles);
        }

        private void ConstructShards(List<Triangle> triangles)
        {
            foreach (var t in triangles)
            {
                var center = new Vector2((t.P1.x + t.P2.x + t.P3.x) / 3.0f, (t.P1.y + t.P2.y + t.P3.y) / 3.0f);
                var shard = ConstructShard(_playerSprite.Texture, t, center);
                CallDeferred(nameof(AddShard), shard);
            }
        }

        private void AddShard(RigidBody2D shard)
        {
            GD.Randomize();
            var dir = Vector2.Up.Rotated((float)GD.RandRange(0, 2 * Mathf.Pi));
            var impulse = (float)GD.RandRange(_minImpulse, _maxImpulse);
            shard.ApplyCentralImpulse(dir * impulse);
            shard.GetChild<CollisionPolygon2D>(1).Disabled = false;

            shard.SetAsToplevel(true);

            _playerSprite.AddChild(shard);
        }

        private RigidBody2D ConstructShard(Texture texture, Triangle t, Vector2 pos)
        {
            var shard = new RigidBody2D();
            var polygon2d = new Polygon2D();
            var colPolygon = new CollisionPolygon2D();

            shard.Sleeping = true;
            colPolygon.Disabled = true;

            shard.Position = pos;

            Vector2[] poly = new Vector2[]
            {
                new Vector2(t.P1.x, t.P1.y),
                new Vector2(t.P2.x, t.P2.y),
                new Vector2(t.P3.x, t.P3.y)
            };

            polygon2d.Polygon = poly;
            // polygon2d.Texture = texture;

            polygon2d.Position = -pos;

            var shrunkTris = Geometry.OffsetPolygon2d(poly, -2f);

            if (shrunkTris.Count > 0)
                colPolygon.Polygon = (Vector2[])shrunkTris[0];
            else
                colPolygon.Polygon = poly;

            colPolygon.GlobalPosition = -pos;

            shard.SetCollisionLayerBit(1, true);
            shard.SetCollisionMaskBit(1, true);

            shard.AddChild(polygon2d);
            shard.AddChild(colPolygon);
            Update();

            return shard;
        }

        private Vector2[] CreatePoints()
        {
            var rect = _playerSprite.GetRect();
            var size = rect.Size;
            size *= 0.5f;
            rect.Size = size;
            var points = new Vector2[4 + _shardAmount];

            // points.Add(rect.Position);
            // points.Add(rect.Position + new Vector2(rect.Size.x, 0));
            // points.Add(rect.Position + new Vector2(0, rect.Size.y));
            // points.Add(rect.End);

            points[0] = rect.Position;
            points[1] = rect.Position + new Vector2(rect.Size.x, 0);
            points[2] = rect.Position + new Vector2(0, rect.Size.y);
            points[3] = rect.End;

            for (int i = 4; i < _shardAmount; i++)
            {
                var randomX = (float)GD.RandRange(0f, rect.Size.x);
                var randomY = (float)GD.RandRange(0f, rect.Size.y);
                var p = rect.Position + new Vector2(randomX, randomY);

                var minX = rect.Position.x + _triangulationThreshold;
                var maxX = rect.End.x + _triangulationThreshold;

                p.x = Mathf.Clamp(p.x, minX, maxX);

                var minY = rect.Position.y + _triangulationThreshold;
                var maxY = rect.End.y + _triangulationThreshold;

                p.y = Mathf.Clamp(p.y, minY, maxY);

                // points.Add(p);
                points[i] = p;
            }

            return points;
        }

        private List<Triangle> CreateTriangles(Vector2[] points)
        {
            var delaunay = Geometry.TriangulateDelaunay2d(points);
            var triangles = new List<Triangle>();
            for (int i = 0; i < delaunay.Length / 3; i++)
            {
                // var triangle = new Tuple<Vector2, Vector2, Vector2>(points[delaunay[i + 2]], points[delaunay[i + 1]], points[delaunay[i]]);
                var triangle = new Triangle(points[delaunay[i + 2]], points[delaunay[i + 1]], points[delaunay[i]]);
                triangles.Add(triangle);
            }

            return triangles;
        }

        private PlayerSprite _playerSprite;
        private List<Triangle> _triangles;
        private List<RigidBody2D> _shards;
        private int _shardAmount = 5;
        private float _triangulationThreshold = 10f;
        private float _minImpulse = 50f;
        private float _maxImpulse = 200f;
        private float _shardLifetime = 5f;
    }

    struct Triangle
    {
        public Vector2 P1, P2, P3;
        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }
    }
}