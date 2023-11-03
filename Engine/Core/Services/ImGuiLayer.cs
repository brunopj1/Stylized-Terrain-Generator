using ImGuiNET;
using OpenTK.Graphics.OpenGL4;

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
            if (ImGui.MenuItem("Toggle wireframe mode"))
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

            if (ImGui.MenuItem("Recompile shaders"))
            {
                _engine.Renderer.RecompileAllShaders();
            }

            ImGui.EndMenu();
        }

        ImGui.EndMainMenuBar();
    }
}
