using ImGuiNET;

namespace Engine.Core.Services;

public delegate void ImGuiOverlay();

public class ImGuiRenderer
{
    internal ImGuiRenderer()
    {
    }

    private readonly List<ImGuiOverlay> _generalOverlays = new();
    private readonly List<ImGuiOverlay> _mainMenuBarOverlays = new();

    public void AddOverlay(ImGuiOverlay overlay)
    {
        _generalOverlays.Add(overlay);
    }

    public void RemoveOverlay(ImGuiOverlay overlay)
    {
        _generalOverlays.Remove(overlay);
    }

    public void AddMainMenuBarOverlay(ImGuiOverlay overlay)
    {
        _mainMenuBarOverlays.Add(overlay);
    }

    public void RemoveMainMenuBarOverlay(ImGuiOverlay overlay)
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
        foreach (var overlay in _generalOverlays)
        {
            overlay();
        }
    }


}
