using Engine.Graphics;
using Engine.Util.EngineProperties.Settings;
using ImGuiNET;

namespace Engine.Util.EngineProperties.Properties;

public class Vector4iProperty : AProperty<Vector4i>
{
    public Vector4iProperty(string name, Vector4i value)
        : base(name, value)
    {
    }

    public IntPropertySettings Settings { get; set; } = new();
    public IntPropertyRenderSettings RenderSettings { get; set; } = new();

    protected override void ApplyValueSettings()
    {
        var x = MathHelper.Clamp(_value.X, Settings.Min, Settings.Max);
        var y = MathHelper.Clamp(_value.Y, Settings.Min, Settings.Max);
        var z = MathHelper.Clamp(_value.Z, Settings.Min, Settings.Max);
        var w = MathHelper.Clamp(_value.W, Settings.Min, Settings.Max);
        _value = new(x, y, z, w);
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
            if (ImGui.DragInt4(_name, ref tempValue.X, RenderSettings.DragStep, Settings.Min, Settings.Max))
            {
                Value = tempValue;
            }
        }
        else
        {
            if (ImGui.InputInt4(_name, ref tempValue.X))
            {
                Value = tempValue;
            }
        }
    }
}
