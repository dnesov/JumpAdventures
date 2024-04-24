using System.Collections.Generic;
using System.Linq;

namespace Jump.Utils.Animation
{
    public interface IKeyframe
    {
        float StartTime { get; set; }
    }
    public abstract class Animation<T> where T : IKeyframe
    {
        /// <summary>
        /// Total duration of the animation.
        /// </summary>
        public float Duration => _duration;

        public IReadOnlyList<T> Keyframes => _timeline;
        public int KeyframeCount => Keyframes.Count;
        public T FirstKeyframe => Keyframes[0];

        /// <summary>
        /// The last animation keyframe. Will return a default value of T if the timeline is empty.
        /// </summary>
        public T LastKeyframe => IsEmpty ? default : Keyframes[KeyframeCount - 1];
        public bool IsEmpty => Keyframes.Count == 0;


        /// <summary>
        /// Adds the keyframe at a specified time and updates the timeline.
        /// </summary>
        /// <param name="atTime">Time to add the keyframe at.</param>
        /// <param name="keyframe">The keyframe.</param>
        public void AddKeyframe(float atTime, T keyframe)
        {
            keyframe.StartTime = atTime;
            _timeline.Add(keyframe);
            UpdateTimeline();
        }

        /// <summary>
        /// Removes the keyframe by a specified index.
        /// </summary>
        /// <param name="idx">Keyframe index.</param>
        public void RemoveKeyframe(int idx)
        {
            if (IsEmpty) return;
            _timeline.RemoveAt(idx);
            UpdateTimeline();
        }

        /// <summary>
        /// Gets the keyframe at a specified index.
        /// </summary>
        /// <param name="idx">Keyframe index.</param>
        /// <returns>The keyframe at the index.</returns>
        public T KeyframeAtIdx(int idx) => _timeline[idx];

        /// <summary>
        /// Updates the timeline with the new duration and sorted keyframes. 
        /// </summary>
        public void UpdateTimeline()
        {
            if (IsEmpty) return;
            _timeline = _timeline.OrderBy(x => x.StartTime).ToList();
            _duration = LastKeyframe.StartTime;
        }

        /// <summary>
        /// Seeks the animation at the specified time.
        /// </summary>
        /// <param name="at">Time to seek at.</param>
        /// <returns>An interpolated keyframe T.</returns>
        public T Seek(float at)
        {
            if (KeyframeCount == 1)
            {
                return FirstKeyframe;
            }

            T start = default;
            T end = default;

            // If we failed to get bounding keys, return the only one present.
            if (!GetBoundingKeyframes(at, out start, out end))
            {
                return FirstKeyframe;
            }

            float timeSinceStart = at - start.StartTime;
            float clipDuration = end.StartTime - start.StartTime;
            float t = Clamp(timeSinceStart / clipDuration, 0.0f, 1.0f);

            float tEased = Ease(t);

            T interpolated = default;
            interpolated = Lerp(in start, in end, tEased);

            return interpolated;
        }

        /// <summary>
        /// Gets the closest and farthest keyframe at the specified time.
        /// </summary>
        /// <param name="at">Time to sample from.</param>
        /// <param name="start">Closest keyframe at specified time.</param>
        /// <param name="end">Farthest keyframe at specified time.</param>
        /// <returns>A bool indicating if bounding keyframes were found.</returns>
        protected bool GetBoundingKeyframes(float at, out T start, out T end)
        {
            start = default;
            end = default;

            if (IsEmpty) return false;
            if (KeyframeCount == 1) return false;

            for (int i = 0; i < KeyframeCount - 1; i++)
            {
                var keyframe = _timeline[i];

                if (at >= keyframe.StartTime)
                {
                    start = keyframe;
                    end = _timeline[i + 1];
                }
            }

            return true;
        }

        /// <summary>
        /// Linearly interpolate between two keyframes given the time.
        /// </summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Final value.</param>
        /// <param name="t">Tine parameter.</param>
        /// <returns>An interpolated keyframe.</returns>
        protected abstract T Lerp(in T from, in T to, float t);

        /// <summary>
        /// Easing function for the time.
        /// </summary>
        /// <param name="t">Time parameter</param>
        /// <returns>Eased time.</returns>
        protected virtual float Ease(float t)
        {
            return t;
        }

        /// <summary>
        /// Ensures the value stays between the specified minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Clamped value.</returns>
        private float Clamp(float value, float min, float max)
        {
            float result = value;

            if (result > max)
            {
                result = max;
            }

            if (result < min)
            {
                result = min;
            }

            return result;
        }

        private List<T> _timeline = new();
        private float _duration = 0.0f;
    }
}