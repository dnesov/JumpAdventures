namespace GodotFmod
{
    public interface IParametrizable
    {
        public void SetParameter(string name, float value, bool ignoreSeekSpeed = false);
        public void SetParameter(string name, string label, bool ignoreSeekSpeed = false);
    }
}