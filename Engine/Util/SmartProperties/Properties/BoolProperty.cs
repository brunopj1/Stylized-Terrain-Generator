using Engine.Graphics;
using Engine.Util.SmartProperties.Settings;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class BoolProperty : AProperty<bool>
{
    public BoolProperty(PropertyGroup group, string name, bool? value = null)
        : base(group, name, value)
    {
    }

    public IntPropertyRange Range { get; set; } = new();
    public IntPropertyRenderSettings RenderSettings { get; set; } = new();

    public override string StringValue
    {
        get => Value.ToString();
        set => Value = bool.Parse(value!);
    }

    protected override void ApplyValueSettings()
    {
    }

    internal override void BindUniform(AShader shader)
    {
        shader.BindUniform(UniformName, _value);
    }

    internal override bool RenderInputField()
    {
        var tempValue = _value;

        if (ImGui.Checkbox(Name, ref tempValue))
        {
            Value = tempValue;
            return true;
        }

        return false;
    }
}
