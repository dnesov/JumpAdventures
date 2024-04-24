namespace GodotFmod
{
    public interface IParametrizable
    {
        void SetParameter(string name, float value, bool ignoreSeekSpeed = false);
        void SetParameter(string name, string label, bool ignoreSeekSpeed = false);
    }
}