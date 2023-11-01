using ImGuiNET;

namespace Engine.Core.Services;

internal class ImGuiLayer
{
    public ImGuiLayer(AEngineBase engine)
    {
        _engine = engine;
    }

    private readonly AEngineBase _engine;

    public void RenderMenuBar()
    {
        ImGui.BeginMainMenuBar();

        if (ImGui.BeginMenu("Engine"))
        {
            if (ImGui.MenuItem("Recompile shaders"))
            {
                _engine.Renderer.RecompileShaders();
            }

            ImGui.EndMenu();
        }

        ImGui.EndMainMenuBar();
    }
}
