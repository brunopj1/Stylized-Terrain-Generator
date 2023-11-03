using Engine.Core;
using Engine.Core.Controllers;
using Engine.Graphics;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using TerrainGenerator.Vertices;

namespace Test;

internal class TestEngine : AEngineBase
{
    public TestEngine()
        : base()
    {
        Size = new(1600, 900);
        ClientLocation = new(50, 50);
        Title = "Terrain Generator";
        ClearColor = new(0.2f, 0.3f, 0.3f);

        // Shaders
        // TODO use relative pahts (maybe copy assets to bin folder?)
        var shader1 = Renderer.CreateShader
        (
            @"C:\Projects\OpenGL-Stuff\Projects\Test\Assets\Shaders\triangle.vert",
            @"C:\Projects\OpenGL-Stuff\Projects\Test\Assets\Shaders\triangle.frag"
        );

        var shader2 = Renderer.CreateShader
        (
            @"C:\Projects\OpenGL-Stuff\Projects\Test\Assets\Shaders\box.vert",
            @"C:\Projects\OpenGL-Stuff\Projects\Test\Assets\Shaders\box.frag"
        );

        // Textures
        var boxTexture = Renderer.CreateTexture(@"C:\Projects\OpenGL-Stuff\Projects\Test\Assets\Textures\box.jpg");
        var smileTexture = Renderer.CreateTexture(@"C:\Projects\OpenGL-Stuff\Projects\Test\Assets\Textures\smile.png");

        // Axis
        var axisVertices = new TriangleVertex[]
        {
            new TriangleVertex{ Position = new( 0f, 0f, 0f), Color = new(1f, 0f, 0f) },
            new TriangleVertex{ Position = new( 1f, 0f, 0f), Color = new(1f, 0f, 0f) },
            new TriangleVertex{ Position = new( 0f, 0f, 0f), Color = new(0f, 1f, 0f) },
            new TriangleVertex{ Position = new( 0f, 1f, 0f), Color = new(0f, 1f, 0f) },
            new TriangleVertex{ Position = new( 0f, 0f, 0f), Color = new(0f, 0f, 1f) },
            new TriangleVertex{ Position = new( 0f, 0f, 1f), Color = new(0f, 0f, 1f) }
        };
        var axisMesh = Renderer.CreateMesh(axisVertices, TriangleVertex.GetLayout(), new() { PrimitiveType = PrimitiveType.Lines });
        var axisModel = Renderer.CreateModel(axisMesh, shader1);
        axisModel.BoundingVolume = new BoundingSphere(new(0.5f, 0.5f, 0.5f), 0.5f);

        // Triangle
        var triangleVertices = new TriangleVertex[]
        {
            new TriangleVertex{ Position = new( -1f, -1f, 0f), Color = new(1f, 0f, 0f) },
            new TriangleVertex{ Position = new(  1f, -1f, 0f), Color = new(0f, 1f, 0f) },
            new TriangleVertex{ Position = new(  0f,  1f, 0f), Color = new(0f, 0f, 1f) }
        };
        var triangleMesh = Renderer.CreateMesh(triangleVertices, TriangleVertex.GetLayout());
        var triangleModel = Renderer.CreateModel(triangleMesh, shader1);
        triangleModel.Transform.Position = new(15, 0, 0);
        triangleModel.Transform.Scale = new(10);
        triangleModel.BoundingVolume = new AxisAlignedBoundingBox(new(-1, -1, 0), new(1, 1, 0));

        // Square
        var squareVertices = new BoxVertex[]
        {
            new BoxVertex{ Position = new( -1f, -1f, 0f), TexCoord = new(0f, 0f) },
            new BoxVertex{ Position = new(  1f, -1f, 0f), TexCoord = new(1f, 0f) },
            new BoxVertex{ Position = new( -1f,  1f, 0f), TexCoord = new(0f, 1f) },
            new BoxVertex{ Position = new(  1f,  1f, 0f), TexCoord = new(1f, 1f) }
        };
        var squareIndices = new uint[] { 0, 1, 2, 1, 3, 2 };
        var squareMesh = Renderer.CreateMesh(squareVertices, squareIndices, BoxVertex.GetLayout());
        var squareModel = Renderer.CreateModel(squareMesh, shader2);
        squareModel.Textures.Add(new(boxTexture, "texture0"));
        squareModel.Textures.Add(new(smileTexture, "texture1"));
        squareModel.Transform.Position = new(-15, 0, 0);
        squareModel.Transform.Scale = new(10);
        squareModel.BoundingVolume = new AxisAlignedBoundingBox(new(-1, -1, 0), new(1, 1, 0));

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

        Renderer.RenderAllModels();

        ImGui.ShowDemoWindow();

        CompleteOnRenderFrame();
    }
}
