using Engine.Core;
using Engine.Core.Controllers;
using Engine.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Test;

internal class TestEngine : AEngineBase
{
    private readonly Model<TriangleVertex> _triangleModel;
    private readonly Model<TriangleVertex> _axisModel;

    // TODO move this to the OnLoad method
    public TestEngine()
        : base()
    {
        Size = new(800, 600);
        Title = "Terrain Generator";
        ClearColor = new(0.2f, 0.3f, 0.3f);

        // Shader
        // TODO use relative pahts (maybe copy assets to bin folder?)
        var shader = new Shader
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
        var axisMesh = new Mesh<TriangleVertex>(axisVertices, TriangleVertex.GetLayout(), PrimitiveType.Lines);
        _axisModel = new(axisMesh, shader);

        // Triangle
        var triangleVertices = new TriangleVertex[]
        {
            new TriangleVertex{ Position = new( -10f, -10f, 0.0f), Color = new(1.0f, 0.0f, 0.0f) },
            new TriangleVertex{ Position = new(  10f, -10f, 0.0f), Color = new(0.0f, 1.0f, 0.0f) },
            new TriangleVertex{ Position = new(  0.0f,  10f, 0.0f), Color = new(0.0f, 0.0f, 1.0f) }
        };
        var triangleMesh = new Mesh<TriangleVertex>(triangleVertices, TriangleVertex.GetLayout());
        _triangleModel = new(triangleMesh, shader);

        // Camera
        Camera.Position = new(0.0f, 0.0f, 30.0f);

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

        _axisModel!.Render(UniformManager);
        _triangleModel!.Render(UniformManager);

        SwapBuffers();
    }
}
