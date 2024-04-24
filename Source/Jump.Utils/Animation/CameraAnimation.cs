using Godot;

namespace Jump.Utils.Animation
{
    public class CameraKeyframe : IKeyframe
    {
        public float StartTime { get; set; }

        public Vector2 Position;
        public Vector2 Zoom;
    }
    public class CameraAnimation : Animation<CameraKeyframe>
    {
        protected override CameraKeyframe Lerp(in CameraKeyframe from, in CameraKeyframe to, float t)
        {
            var result = new CameraKeyframe();
            result.Position = from.Position.LinearInterpolate(to.Position, t);
            result.Zoom = from.Zoom.LinearInterpolate(to.Zoom, t);

            return result;
        }

        protected override float Ease(float t)
        {
            return EaseInOutSine(t);
        }

        private float EaseInOutSine(float t)
        {
            return -(Mathf.Cos(Mathf.Pi * t) - 1) / 2;
        }
    }
}