using Engine.Extensions;
using Engine.Graphics;
using Engine.Util.EngineProperties.Settings;
using ImGuiNET;

namespace Engine.Util.EngineProperties.Properties;

public class Vector2Property : AProperty<Vector2>
{
    public Vector2Property(string name, Vector2 value)
        : base(name, value)
    {
    }

    public FloatPropertySettings Settings { get; set; } = new();
    public FloatPropertyRenderSettings RenderSettings { get; set; } = new();

    protected override void ApplyValueSettings()
    {
        var x = MathHelper.Clamp(_value.X, Settings.Min, Settings.Max);
        var y = MathHelper.Clamp(_value.Y, Settings.Min, Settings.Max);
        _value = new(x, y);
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
            if (ImGui.DragFloat2(_name, ref tempValue, RenderSettings.DragStep, Settings.Min, Settings.Max, RenderSettings.Format))
            {
                Value = tempValue.ToOpenTK();
            }
        }
        else
        {
            if (ImGui.InputFloat2(_name, ref tempValue, RenderSettings.Format))
            {
                Value = tempValue.ToOpenTK();
            }
        }
    }
}
