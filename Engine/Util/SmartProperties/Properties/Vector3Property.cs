using Engine.Extensions;
using Engine.Graphics;
using Engine.Helpers;
using Engine.Util.SmartProperties.Settings;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class Vector3Property : AProperty<Vector3>
{
    public Vector3Property(PropertyGroup group, string name, Vector3? value = null)
        : base(group, name, value)
    {
    }

    public FloatPropertyRange Range { get; set; } = new();
    public FloatPropertyRenderSettings RenderSettings { get; set; } = new();

    public override string StringValue
    {
        get => Value.ToString();
        set => Value = VectorHelper.ParseVector3(value!);
    }

    protected override void ApplyValueSettings()
    {
        var x = MathHelper.Clamp(_value.X, Range.Min, Range.Max);
        var y = MathHelper.Clamp(_value.Y, Range.Min, Range.Max);
        var z = MathHelper.Clamp(_value.Z, Range.Min, Range.Max);
        _value = new(x, y, z);
    }

    internal override void BindUniform(AShader shader)
    {
        shader.BindUniform(UniformName, _value);
    }

    internal override bool RenderInputField()
    {
        var tempValue = _value.ToNumerics();

        if (RenderSettings.EnableDrag)
        {
            if (ImGui.DragFloat3(Name, ref tempValue, RenderSettings.DragStep, Range.Min, Range.Max, RenderSettings.Format))
            {
                Value = tempValue.ToOpenTK();
                return true;
            }

            return false;
        }
        else
        {
            if (ImGui.InputFloat3(Name, ref tempValue, RenderSettings.Format))
            {
                Value = tempValue.ToOpenTK();
                return true;
            }

            return false;
        }
    }
}
