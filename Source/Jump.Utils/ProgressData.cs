namespace Jump.Utils
{
    public struct ProgressData
    {
        public CollectibleProgressData Collectibles;
        public WorldProgressData WorldProgress;
    }
    public struct CollectibleProgressData
    {
        public int Essence;
        public int Fragments;
    }

    public struct WorldProgressData
    {
        public int LevelsCompleted;
        public float PercentageComplete;
    }
}