using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties.Other;
public class PropertyPushItemWidth : ACustomProperty
{
    public PropertyPushItemWidth(PropertyGroup group, float width)
        : base(group)
    {
        Width = width;
    }

    public float Width { get; set; }

    internal override bool RenderInputField()
    {
        ImGui.PushItemWidth(Width);
        return false;
    }
}

public class PropertyPopItemWidth : ACustomProperty
{
    public PropertyPopItemWidth(PropertyGroup group)
        : base(group)
    {
    }

    internal override bool RenderInputField()
    {
        ImGui.PopItemWidth();
        return false;
    }
}
