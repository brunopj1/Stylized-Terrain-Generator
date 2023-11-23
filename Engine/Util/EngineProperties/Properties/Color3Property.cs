using Engine.Extensions;
using Engine.Graphics;
using ImGuiNET;

namespace Engine.Util.EngineProperties.Properties;

public class Color3Property : AProperty<Vector3>
{
    public Color3Property(string name, Vector3 value)
        : base(name, value)
    {
    }

    protected override void ApplyValueSettings()
    {
        var x = MathHelper.Clamp(_value.X, 0.0f, 1.0f);
        var y = MathHelper.Clamp(_value.Y, 0.0f, 1.0f);
        var z = MathHelper.Clamp(_value.Z, 0.0f, 1.0f);
        _value = new(x, y, z);
    }

    public override void BindUniform(AShader shader)
    {
        shader.BindUniform(_uniformName, _value);
    }

    public override void RenderInputField()
    {
        var tempValue = _value.ToNumerics();

        if (ImGui.ColorEdit3(_name, ref tempValue))
        {
            Value = tempValue.ToOpenTK();
        }
    }
}
