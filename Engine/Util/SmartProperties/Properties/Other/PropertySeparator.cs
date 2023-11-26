using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties.Other;

public class PropertySeparator : ACustomProperty
{
    public PropertySeparator(PropertyGroup group, uint count = 1)
        : base(group)
    {
        Count = count;
    }

    public uint Count { get; set; }

    internal override bool RenderInputField()
    {
        for (var i = 0; i < Count; i++)
        {
            ImGui.Separator();
        }

        return false;
    }
}
