using Engine.Graphics;
using Engine.Util.SmartProperties.Settings;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class BoolProperty : AProperty<bool>
{
    public BoolProperty(PropertyGroup group, string name, bool value)
        : base(group, name, value)
    {
    }

    public IntPropertyRange Range { get; set; } = new();
    public IntPropertyRenderSettings RenderSettings { get; set; } = new();

    protected override void ApplyValueSettings()
    {
    }

    public override void BindUniform(AShader shader)
    {
        shader.BindUniform(_uniformName, _value);
    }

    public override bool RenderInputField()
    {
        var tempValue = _value;

        if (ImGui.Checkbox(_name, ref tempValue))
        {
            Value = tempValue;
            return true;
        }

        return false;
    }
}
