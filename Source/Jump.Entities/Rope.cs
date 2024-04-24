using System;
using System.Collections.Generic;
using Godot;

namespace Jump.Entities
{
    public class Rope : Line2D
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            CreatePoints();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            Simulate(delta);
            // ApplyConstraints();
        }

        private void CreatePoints()
        {
            _points = new RopePoint[_pointCount];

            var startPos = Position;
            for (int i = 0; i < _pointCount; i++)
            {
                startPos.y += _pointDistance;

                bool isStationary = i == 0;
                var point = new RopePoint(startPos, isStationary);
                _points[i] = point;

                AddPoint(startPos);
            }
        }

        private void Simulate(float delta)
        {
            for (int i = 0; i < _pointCount; i++)
            {
                var point = _points[i];
                if (point.Stationary) continue;
                var prevPos = point.CurrentPosition;
                var vel = point.CurrentPosition - point.PreviousPosition;

                point.CurrentPosition += vel;
                point.CurrentPosition += Vector2.Down * _gravity * delta * delta;
                point.PreviousPosition = prevPos;

                UpdateSegment(point, i);
            }
        }

        private void ApplyConstraints()
        {
            var firstPoint = _points[0];
            firstPoint.CurrentPosition = GlobalPosition;

            for (int i = 0; i < _pointCount - 1; i++)
            {
                var point = _points[i];
                var nextPoint = _points[i + 1];

                float dist = point.CurrentPosition.DistanceTo(nextPoint.CurrentPosition);
                float error = dist - _pointDistance;

                var changeDir = point.CurrentPosition.DirectionTo(nextPoint.CurrentPosition).Normalized();
                var changeAmount = changeDir * error;

                point.CurrentPosition -= changeAmount * 0.5f;

                nextPoint.CurrentPosition += changeAmount * 0.5f;

                _points[i] = point;
                _points[i + 1] = nextPoint;
            }
        }

        private void UpdateSegment(RopePoint point, int idx)
        {
            var pos = point.CurrentPosition;
            SetPointPosition(idx, pos);
        }


        [Export] private int _pointCount = 30;
        [Export] private float _pointDistance = 0.25f;
        [Export] private float _gravity = 9.8f;
        private RopePoint[] _points;
    }

    public class RopePoint
    {
        public RopePoint(Vector2 position, bool stationary)
        {
            CurrentPosition = position;
            PreviousPosition = position;
            Stationary = stationary;
        }

        public Vector2 CurrentPosition;
        public Vector2 PreviousPosition;
        public bool Stationary;
    }
}
