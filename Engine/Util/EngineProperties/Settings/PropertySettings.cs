namespace Engine.Util.EngineProperties.Settings;

public struct IntPropertySettings
{
    public IntPropertySettings()
    {
    }

    public int Min { get; set; } = int.MinValue;
    public int Max { get; set; } = int.MaxValue;
}

public struct FloatPropertySettings
{
    public FloatPropertySettings()
    {
    }

    public float Min { get; set; } = float.MinValue;
    public float Max { get; set; } = float.MaxValue;
}
