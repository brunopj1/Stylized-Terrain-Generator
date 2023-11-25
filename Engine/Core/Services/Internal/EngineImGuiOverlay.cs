using ImGuiNET;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Core.Services.Internal;

internal class EngineImGuiOverlay
{
    public EngineImGuiOverlay(AEngineBase engine, Action onRecompileShaders)
    {
        _engine = engine;
        _onRecompileShaders = onRecompileShaders;

        _engine.ImGuiRenderer.AddMainMenuBarOverlay(Render);
    }

    private readonly AEngineBase _engine;

    private bool _recompileShaders = false;
    private readonly Action _onRecompileShaders;

    public void Render()
    {
        if (ImGui.BeginMenu("Engine"))
        {
            if (ImGui.MenuItem("Toggle VSync", "F2"))
            {
                ToggleVsync();
            }

            if (ImGui.MenuItem("Toggle wireframe mode", "F3"))
            {
                ToggleWireframeMode();
            }

            if (ImGui.MenuItem("Recompile shaders", "F5"))
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
            ToggleVsync();
        }

        if (keyboardState.IsKeyPressed(Keys.F3))
        {
            ToggleWireframeMode();
        }

        if (keyboardState.IsKeyPressed(Keys.F5) || _recompileShaders)
        {
            RecompileShaders();
            _recompileShaders = false;
        }
    }

    private void ToggleVsync()
    {
        var vsync = _engine.VSync;

        if (vsync == VSyncMode.On)
        {
            _engine.VSync = VSyncMode.Off;
        }
        else
        {
            _engine.VSync = VSyncMode.On;
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
        _onRecompileShaders();
    }
}
