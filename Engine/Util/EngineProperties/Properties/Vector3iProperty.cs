using Engine.Graphics;
using Engine.Util.EngineProperties.Settings;
using ImGuiNET;

namespace Engine.Util.EngineProperties.Properties;

public class Vector3iProperty : AProperty<Vector3i>
{
    public Vector3iProperty(string name, Vector3i value)
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
        _value = new(x, y, z);
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
            if (ImGui.DragInt3(_name, ref tempValue.X, RenderSettings.DragStep, Settings.Min, Settings.Max))
            {
                Value = tempValue;
            }
        }
        else
        {
            if (ImGui.InputInt3(_name, ref tempValue.X))
            {
                Value = tempValue;
            }
        }
    }
}
