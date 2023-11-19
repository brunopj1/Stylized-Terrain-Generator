using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Core.Services.Internal;

internal class EngineImGuiOverlay
{
    public EngineImGuiOverlay(AEngineBase engine)
    {
        _engine = engine;

        _engine.ImGuiRenderer.AddMainMenuBarOverlay(Render);
    }

    private readonly AEngineBase _engine;

    private bool _recompileShaders = false;

    public void Render()
    {
        if (ImGui.BeginMenu("Engine"))
        {
            if (ImGui.MenuItem("Toggle wireframe mode", "F2"))
            {
                ToggleWireframeMode();
            }

            if (ImGui.MenuItem("Recompile shaders", "F3"))
            {
                // Delayed recompilation to avoid ImGui crash
                _recompileShaders = true;
            }

            ImGui.EndMenu();
        }
    }

    public void ProcessInputs(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyPressed(Keys.F2))
        {
            ToggleWireframeMode();
        }

        if (keyboardState.IsKeyPressed(Keys.F3) || _recompileShaders)
        {
            RecompileShaders();
            _recompileShaders = false;
        }
    }

    private static void ToggleWireframeMode()
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
