using Engine.Extensions;
using Engine.Graphics;
using Engine.Helpers;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class Color4Property : AProperty<Vector4>
{
    public Color4Property(PropertyGroup group, string name, Vector4? value = null)
        : base(group, name, value)
    {
    }

    public override string StringValue
    {
        get => Value.ToString();
        set => Value = VectorHelper.ParseVector4(value!);
    }

    protected override void ApplyValueSettings()
    {
        var x = MathHelper.Clamp(_value.X, 0.0f, 1.0f);
        var y = MathHelper.Clamp(_value.Y, 0.0f, 1.0f);
        var z = MathHelper.Clamp(_value.Z, 0.0f, 1.0f);
        var w = MathHelper.Clamp(_value.W, 0.0f, 1.0f);
        _value = new(x, y, z, w);
    }

    internal override void BindUniform(AShader shader)
    {
        shader.BindUniform(UniformName, _value);
    }

    internal override bool RenderInputField()
    {
        var tempValue = _value.ToNumerics();

        if (ImGui.ColorEdit4(Name, ref tempValue))
        {
            Value = tempValue.ToOpenTK();
            return true;
        }

        return false;
    }
}
