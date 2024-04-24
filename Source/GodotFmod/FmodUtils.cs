using Godot;

namespace GodotFmod
{
    public static class FmodUtils
    {
        public static FMOD.VECTOR GodotVectorToFmod(Vector3 vector)
        {
            return new FMOD.VECTOR()
            {
                x = vector.x,
                y = vector.y,
                z = vector.z
            };
        }
    }
}