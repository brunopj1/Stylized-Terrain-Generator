using Engine.Graphics;
using Engine.Util.SmartProperties.Settings;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class Vector2iProperty : AProperty<Vector2i>
{
    public Vector2iProperty(PropertyGroup group, string name, Vector2i value)
        : base(group, name, value)
    {
    }

    public IntPropertyRange Range { get; set; } = new();
    public IntPropertyRenderSettings RenderSettings { get; set; } = new();

    protected override void ApplyValueSettings()
    {
        var x = MathHelper.Clamp(_value.X, Range.Min, Range.Max);
        var y = MathHelper.Clamp(_value.Y, Range.Min, Range.Max);
        _value = new(x, y);
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
            if (ImGui.DragInt2(_name, ref tempValue.X, RenderSettings.DragStep, Range.Min, Range.Max))
            {
                Value = tempValue;
            }
        }
        else
        {
            if (ImGui.InputInt2(_name, ref tempValue.X))
            {
                Value = tempValue;
            }
        }
    }
}
