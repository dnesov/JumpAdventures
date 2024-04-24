using YamlDotNet.Serialization;

namespace Jump.Levels
{
    public class Level
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool ShowIntro { get; set; } = true;
        public int MaxEssence { get; set; } = 0;

        [YamlIgnore] public string GlobalPath { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Path})";
        }
    }
}