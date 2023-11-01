using Engine.Core;
using Engine.Core.Controllers;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Test;

internal class TestEngine : AEngineBase
{
    public TestEngine()
        : base()
    {
        Size = new(1600, 900);
        Title = "Terrain Generator";
        ClearColor = new(0.2f, 0.3f, 0.3f);

        // Shader
        // TODO use relative pahts (maybe copy assets to bin folder?)
        var shader = Renderer.CreateShader
        (
            @"C:\Projects\OpenGL-Stuff\Projects\Test\Assets\Shaders\triangle.vert",
            @"C:\Projects\OpenGL-Stuff\Projects\Test\Assets\Shaders\triangle.frag"
        );

        // Axis
        var axisVertices = new TriangleVertex[]
        {
            new TriangleVertex{ Position = new( 0.0f, 0.0f, 0.0f), Color = new(1.0f, 0.0f, 0.0f) },
            new TriangleVertex{ Position = new( 1.0f, 0.0f, 0.0f), Color = new(1.0f, 0.0f, 0.0f) },
            new TriangleVertex{ Position = new( 0.0f, 0.0f, 0.0f), Color = new(0.0f, 1.0f, 0.0f) },
            new TriangleVertex{ Position = new( 0.0f, 1.0f, 0.0f), Color = new(0.0f, 1.0f, 0.0f) },
            new TriangleVertex{ Position = new( 0.0f, 0.0f, 0.0f), Color = new(0.0f, 0.0f, 1.0f) },
            new TriangleVertex{ Position = new( 0.0f, 0.0f, 1.0f), Color = new(0.0f, 0.0f, 1.0f) }
        };
        var axisMesh = Renderer.CreateMesh(axisVertices, TriangleVertex.GetLayout(), primitiveType: PrimitiveType.Lines);
        Renderer.CreateModel(axisMesh, shader);

        // Triangle
        var triangleVertices = new TriangleVertex[]
        {
            new TriangleVertex{ Position = new( -10f, -10f, 0.0f), Color = new(1.0f, 0.0f, 0.0f) },
            new TriangleVertex{ Position = new(  10f, -10f, 0.0f), Color = new(0.0f, 1.0f, 0.0f) },
            new TriangleVertex{ Position = new(  0.0f,  10f, 0.0f), Color = new(0.0f, 0.0f, 1.0f) }
        };
        var triangleMesh = Renderer.CreateMesh(triangleVertices, TriangleVertex.GetLayout());
        Renderer.CreateModel(triangleMesh, shader);

        // Camera
        Renderer.Camera.Position = new(0.0f, 0.0f, 30.0f);

        // Player Controller
        PlayerController = new DefaultPlayerController(this);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        VSync = VSyncMode.On;
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        Renderer.RenderAll();

        ImGui.ShowDemoWindow();

        CompleteOnRenderFrame();
    }
}
