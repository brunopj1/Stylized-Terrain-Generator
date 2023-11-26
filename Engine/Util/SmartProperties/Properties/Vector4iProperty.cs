using Engine.Graphics;
using Engine.Helpers;
using Engine.Util.SmartProperties.Settings;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class Vector4iProperty : AProperty<Vector4i>
{
    public Vector4iProperty(PropertyGroup group, string name, Vector4i? value = null)
        : base(group, name, value)
    {
    }

    public IntPropertyRange Range { get; set; } = new();
    public IntPropertyRenderSettings RenderSettings { get; set; } = new();

    public override string StringValue
    {
        get => Value.ToString();
        set => Value = VectorHelper.ParseVector4i(value!);
    }

    protected override void ApplyValueSettings()
    {
        var x = MathHelper.Clamp(_value.X, Range.Min, Range.Max);
        var y = MathHelper.Clamp(_value.Y, Range.Min, Range.Max);
        var z = MathHelper.Clamp(_value.Z, Range.Min, Range.Max);
        var w = MathHelper.Clamp(_value.W, Range.Min, Range.Max);
        _value = new(x, y, z, w);
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
            if (ImGui.DragInt4(Name, ref tempValue.X, RenderSettings.DragStep, Range.Min, Range.Max))
            {
                Value = tempValue;
                return true;
            }

            return false;
        }
        else
        {
            if (ImGui.InputInt4(Name, ref tempValue.X))
            {
                Value = tempValue;
                return true;
            }

            return false;
        }
    }
}
