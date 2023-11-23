using Engine.Graphics;
using Engine.Util.EngineProperties.Settings;
using ImGuiNET;

namespace Engine.Util.EngineProperties.Properties;

public class FloatProperty : AProperty<float>
{
    public FloatProperty(string name, float value)
        : base(name, value)
    {
    }

    public FloatPropertySettings Settings { get; set; } = new();
    public FloatPropertyRenderSettings RenderSettings { get; set; } = new();

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
            if (ImGui.DragFloat(_name, ref tempValue, RenderSettings.DragStep, Settings.Min, Settings.Max))
            {
                Value = tempValue;
            }
        }
        else
        {
            if (ImGui.InputFloat(_name, ref tempValue, RenderSettings.SlowStep, RenderSettings.FastStep, RenderSettings.Format))
            {
                Value = tempValue;
            }
        }
    }
}
