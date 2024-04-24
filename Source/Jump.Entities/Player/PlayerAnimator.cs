using Godot;

namespace Jump.Entities
{
    public class PlayerAnimator : Node
    {
        public void Jump(float dir)
        {
            // _targetRotation = dir * Mathf.Deg2Rad(20);
        }

        public float GetRotation()
        {
            return _targetRotation;
        }

        private void Stand(float delta)
        {
            _targetRotation = _surfaceAngle;
        }

        private void Fall(float delta)
        {
            _targetRotation = _surfaceAngle;
        }

        private float _fallAnim, _jumpAnim, _surfaceAngle;
        private float _lastSurfaceAngle;
        private float _surfaceAngleAccumulator;
        private float _targetRotation;
    }
}