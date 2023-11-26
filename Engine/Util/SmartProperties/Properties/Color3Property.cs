using Engine.Extensions;
using Engine.Graphics;
using Engine.Helpers;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class Color3Property : AProperty<Vector3>
{
    public Color3Property(PropertyGroup group, string name, Vector3? value = null)
        : base(group, name, value)
    {
    }

    public override string StringValue
    {
        get => Value.ToString();
        set => Value = VectorHelper.ParseVector3(value!);
    }

    protected override void ApplyValueSettings()
    {
        var x = MathHelper.Clamp(_value.X, 0.0f, 1.0f);
        var y = MathHelper.Clamp(_value.Y, 0.0f, 1.0f);
        var z = MathHelper.Clamp(_value.Z, 0.0f, 1.0f);
        _value = new(x, y, z);
    }

    internal override void BindUniform(AShader shader)
    {
        shader.BindUniform(UniformName, _value);
    }

    internal override bool RenderInputField()
    {
        var tempValue = _value.ToNumerics();

        if (ImGui.ColorEdit3(Name, ref tempValue))
        {
            Value = tempValue.ToOpenTK();
            return true;
        }

        return false;
    }
}
