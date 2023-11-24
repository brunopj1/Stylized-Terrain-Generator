using Engine.Graphics;
using Engine.Util.SmartProperties.Settings;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class FloatProperty : AProperty<float>
{
    public FloatProperty(PropertyGroup group, string name, float value)
        : base(group, name, value)
    {
    }

    public FloatPropertyRange Range { get; set; } = new();
    public FloatPropertyRenderSettings RenderSettings { get; set; } = new();

    protected override void ApplyValueSettings()
    {
        _value = MathHelper.Clamp(_value, Range.Min, Range.Max);
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
            if (ImGui.DragFloat(_name, ref tempValue, RenderSettings.DragStep, Range.Min, Range.Max))
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
