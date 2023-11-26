using Engine.Graphics;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties.Other;

public class PropertySeparator : AProperty
{
    public PropertySeparator(PropertyGroup group, uint count = 1)
        : base(group, "")
    {
        Count = count;
    }

    public uint Count { get; set; }

    public override string StringValue { get => ""; set { } }

    public override bool AllowSerialization { get => false; set { } }
    public override bool HasShaderUniform { get => false; set { } }

    internal override void BindUniform(AShader shader)
    {
    }

    internal override bool RenderInputField()
    {
        for (var i = 0; i < Count; i++)
        {
            ImGui.Separator();
        }

        return false;
    }
}
