namespace Engine.Util.EngineProperties.Settings;

public struct IntPropertyRenderSettings
{
    public IntPropertyRenderSettings()
    {
    }

    public bool EnableDrag { get; set; } = true;
    public int DragStep { get; set; } = 1;
    public int SlowStep { get; set; } = 1;
    public int FastStep { get; set; } = 10;
}

public struct FloatPropertyRenderSettings
{
    public FloatPropertyRenderSettings()
    {
    }

    public string? Format { get; set; } = null;
    public bool EnableDrag { get; set; } = true;
    public float DragStep { get; set; } = 0.1f;
    public float SlowStep { get; set; } = 0.1f;
    public float FastStep { get; set; } = 1f;
}
