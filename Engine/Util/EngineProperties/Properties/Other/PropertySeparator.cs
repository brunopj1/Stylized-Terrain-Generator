using Engine.Graphics;
using ImGuiNET;

namespace Engine.Util.EngineProperties.Properties.Other;

public class PropertySeparator : IProperty
{
    public PropertySeparator(uint count = 1)
    {
        Count = count;
    }

    public uint Count { get; set; }

    public bool HasShaderUniform { get => false; set { } }
    public bool HasInputField { get => false; set { } }

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
