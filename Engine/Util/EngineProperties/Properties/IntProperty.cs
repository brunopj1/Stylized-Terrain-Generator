using Engine.Graphics;
using Engine.Util.EngineProperties.Settings;
using ImGuiNET;

namespace Engine.Util.EngineProperties.Properties;

public class IntProperty : AProperty<int>
{
    public IntProperty(string name, int value)
        : base(name, value)
    {
    }

    public IntPropertySettings Settings { get; set; } = new();
    public IntPropertyRenderSettings RenderSettings { get; set; } = new();

    protected override void ApplyValueSettings()
    {
        _value = MathHelper.Clamp(_value, Settings.Min, Settings.Max);
    }

    public override void BindUniform(AShader shader)
    {
        shader.BindUniform(_uniformName, _value);
    }

    public override void RenderInputField()
    {
        var tempValue = _value;

        if (RenderSettings.EnableDrag)
        {
            if (ImGui.DragInt(_name, ref tempValue, RenderSettings.DragStep, Settings.Min, Settings.Max))
            {
                Value = tempValue;
            }
        }
        else
        {
            if (ImGui.InputInt(_name, ref tempValue, RenderSettings.SlowStep, RenderSettings.FastStep))
            {
                Value = tempValue;
            }
        }
    }
}
