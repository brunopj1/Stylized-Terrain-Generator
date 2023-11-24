using Engine.Graphics;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties.Other;

public class PropertySeparator : IProperty
{
    public PropertySeparator(PropertyGroup group, uint count = 1)
    {
        group.AddProperty(this);

        Count = count;
    }

    public uint Count { get; set; }

    public bool HasShaderUniform { get => false; set { } }
    public bool HasInputField { get => true; set { } }

    public void BindUniform(AShader shader)
    { }

    public void RenderInputField()
    {
        for (var i = 0; i < Count; i++)
        {
            ImGui.Separator();
        }
    }
}
