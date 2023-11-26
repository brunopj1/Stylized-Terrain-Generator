using Engine.Graphics;
using Engine.Helpers;
using Engine.Util.SmartProperties.Settings;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class Vector3iProperty : AProperty<Vector3i>
{
    public Vector3iProperty(PropertyGroup group, string name, Vector3i? value = null)
        : base(group, name, value)
    {
    }

    public IntPropertyRange Range { get; set; } = new();
    public IntPropertyRenderSettings RenderSettings { get; set; } = new();

    public override string StringValue
    {
        get => Value.ToString();
        set => Value = VectorHelper.ParseVector3i(value!);
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
        var tempValue = _value;

        if (RenderSettings.EnableDrag)
        {
            if (ImGui.DragInt3(Name, ref tempValue.X, RenderSettings.DragStep, Range.Min, Range.Max))
            {
                Value = tempValue;
                return true;
            }

            return false;
        }
        else
        {
            if (ImGui.InputInt3(Name, ref tempValue.X))
            {
                Value = tempValue;
                return true;
            }

            return false;
        }
    }
}
