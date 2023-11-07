using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;

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
            if (ImGui.MenuItem("Toggle wireframe mode", "F2"))
            {
                ToggleWireframeMode();
            }

            if (ImGui.MenuItem("Recompile shaders", "F3"))
            {
                RecompileShaders();
            }

            ImGui.EndMenu();
        }

        ImGui.EndMainMenuBar();
    }

    public void ProcessInputs(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyPressed(Keys.F2))
        {
            ToggleWireframeMode();
        }

        if (keyboardState.IsKeyPressed(Keys.F3))
        {
            RecompileShaders();
        }
    }

    private void ToggleWireframeMode()
    {
        var wireframeMode = GL.GetInteger(GetPName.PolygonMode) != (int)PolygonMode.Line;
        if (wireframeMode)
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Disable(EnableCap.CullFace);
        }
        else
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Enable(EnableCap.CullFace);
        }
    }

    private void RecompileShaders()
    {
        _engine.Renderer.RecompileAllShaders();
    }
}
