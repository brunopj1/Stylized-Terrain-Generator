using Engine.Graphics;
using Engine.Util.SmartProperties.Settings;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class FloatProperty : AProperty<float>
{
    public FloatProperty(PropertyGroup group, string name, float? value = null)
        : base(group, name, value)
    {
    }

    public FloatPropertyRange Range { get; set; } = new();
    public FloatPropertyRenderSettings RenderSettings { get; set; } = new();

    public override string StringValue
    {
        get => Value.ToString();
        set => Value = float.Parse(value!);
    }

    protected override void ApplyValueSettings()
    {
        _value = MathHelper.Clamp(_value, Range.Min, Range.Max);
    }

    internal override void BindUniform(AShader shader)
    {
        shader.BindUniform(UniformName, _value);
    }

    internal override bool RenderInputField()
    {
        var tempValue = _value;

        if (RenderSettings.EnableDrag)
        {
            if (ImGui.DragFloat(Name, ref tempValue, RenderSettings.DragStep, Range.Min, Range.Max))
            {
                Value = tempValue;
                return true;
            }

            return false;
        }
        else
        {
            if (ImGui.InputFloat(Name, ref tempValue, RenderSettings.SlowStep, RenderSettings.FastStep, RenderSettings.Format))
            {
                Value = tempValue;
                return true;
            }

            return false;
        }
    }
}
