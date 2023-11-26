using Engine.Graphics;
using Engine.Util.SmartProperties.Settings;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class IntProperty : AProperty<int>
{
    public IntProperty(PropertyGroup group, string name, int? value = null)
        : base(group, name, value)
    {
    }

    public IntPropertyRange Range { get; set; } = new();
    public IntPropertyRenderSettings RenderSettings { get; set; } = new();

    public override string StringValue
    {
        get => Value.ToString();
        set => Value = int.Parse(value!);
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
            if (ImGui.DragInt(Name, ref tempValue, RenderSettings.DragStep, Range.Min, Range.Max))
            {
                Value = tempValue;
                return true;
            }

            return false;
        }
        else
        {
            if (ImGui.InputInt(Name, ref tempValue, RenderSettings.SlowStep, RenderSettings.FastStep))
            {
                Value = tempValue;
                return true;
            }

            return false;
        }
    }
}
