namespace Engine.Util.SmartProperties.Properties.Other;

public class PropertyCustomRendering : ACustomProperty
{
    public PropertyCustomRendering(PropertyGroup group, ImGuiOverlayHandler onRender)
        : base(group)
    {
        OnImGuiOverlay += onRender;
    }

    public delegate void ImGuiOverlayHandler();
    public event ImGuiOverlayHandler? OnImGuiOverlay;

    internal override bool RenderInputField()
    {
        OnImGuiOverlay?.Invoke();
        return false;
    }
}
