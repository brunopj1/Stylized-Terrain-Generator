using ImGuiNET;

namespace Engine.Core.Services;

public class ImGuiRenderer
{
    internal ImGuiRenderer()
    {
    }

    public delegate void ImGuiOverlayHandler();

    private readonly List<ImGuiOverlayHandler> _windowOverlays = new();
    private readonly List<ImGuiOverlayHandler> _mainMenuBarOverlays = new();

    public void AddWindowOverlay(ImGuiOverlayHandler overlay)
    {
        _windowOverlays.Add(overlay);
    }

    public void RemoveWindowOverlay(ImGuiOverlayHandler overlay)
    {
        _windowOverlays.Remove(overlay);
    }

    public void AddMainMenuBarOverlay(ImGuiOverlayHandler overlay)
    {
        _mainMenuBarOverlays.Add(overlay);
    }

    public void RemoveMainMenuBarOverlay(ImGuiOverlayHandler overlay)
    {
        _mainMenuBarOverlays.Remove(overlay);
    }

    internal void Render()
    {
        // Main menu bar
        if (_mainMenuBarOverlays.Any())
        {
            ImGui.BeginMainMenuBar();

            foreach (var overlay in _mainMenuBarOverlays)
            {
                overlay();
            }

            ImGui.EndMainMenuBar();
        }

        // General
        foreach (var overlay in _windowOverlays)
        {
            overlay();
        }
    }


}
