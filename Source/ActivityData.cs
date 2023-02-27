public struct ActivityData
{
    public string State { get; set; }
    public string Details { get; set; }
    public ActivityTime Time { get; set; }
    public ActivityAssets Assets { get; set; }
}

public struct ActivityAssets
{
    public string LargeAsset { get; set; }
    public string LargeAssetText { get; set; }
    public string SmallAsset { get; set; }
}

public struct ActivityTime
{
    public long Start { get; set; }
    public long End { get; set; }
}