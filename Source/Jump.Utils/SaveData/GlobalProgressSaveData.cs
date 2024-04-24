using Ceras;

namespace Jump.Utils.SaveData
{
    public class GlobalProgressSaveData : SaveDataBase
    {
        [Exclude] public override string FileName => "global_progress.jasv";
        public int Version = 1;
        public int Essence;
        public int Fragments;
        public int Experience;
        public int Attempts;
        public int Jumps;
        public float PlayTime = 0.0f;
    }
}