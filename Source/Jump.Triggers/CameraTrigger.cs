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
            player.Camera.InterpolateTo(target, _smoothingSpeed);
        }

        protected override void OnExited(Player player)
        {
            player.Camera.ReturnToPlayer();
        }

        [Export] private NodePath _targetCamera;
        [Export] private float _smoothingSpeed = 2.0f;
    }
}
