namespace Engine.Util.SmartProperties.Settings;

public struct IntPropertyRange
{
    public IntPropertyRange()
    {
    }

    public int Min { get; set; } = int.MinValue;
    public int Max { get; set; } = int.MaxValue;
}

public struct FloatPropertyRange
{
    public FloatPropertyRange()
    {
    }

    public float Min { get; set; } = float.MinValue;
    public float Max { get; set; } = float.MaxValue;
}
