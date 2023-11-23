using Engine.Extensions;
using Engine.Graphics;
using Engine.Util.EngineProperties.Settings;
using ImGuiNET;

namespace Engine.Util.EngineProperties.Properties;

public class Vector4Property : AProperty<Vector4>
{
    public Vector4Property(string name, Vector4 value)
        : base(name, value)
    {
    }

    public FloatPropertySettings Settings { get; set; } = new();
    public FloatPropertyRenderSettings RenderSettings { get; set; } = new();

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
        var tempValue = _value.ToNumerics();

        if (RenderSettings.EnableDrag)
        {
            if (ImGui.DragFloat4(_name, ref tempValue, RenderSettings.DragStep, Settings.Min, Settings.Max, RenderSettings.Format))
            {
                Value = tempValue.ToOpenTK();
            }
        }
        else
        {
            if (ImGui.InputFloat4(_name, ref tempValue, RenderSettings.Format))
            {
                Value = tempValue.ToOpenTK();
            }
        }
    }
}
