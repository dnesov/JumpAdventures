using Godot;
using Jump.Entities;
using System;

namespace Jump.Triggers
{
    public class CameraTrigger : BaseTrigger
    {
        protected override void OnEntered(Player player)
        {
            var target = GetNode<Camera2D>(_targetCamera);
            player.Camera.InterpolateTo(target, _smoothingSpeedScale);
        }

        protected override void OnExited(Player player)
        {
            player.Camera.ReturnToPlayer();
        }

        [Export] private NodePath _targetCamera;
        [Export] private float _smoothingSpeedScale = 0.5f;
    }
}
