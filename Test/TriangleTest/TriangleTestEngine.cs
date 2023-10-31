using OpenTK.Windowing.Common;
using Engine.Graphics;
using OpenTK.Graphics.OpenGL4;
using TerrainGenerator;
using System.Runtime.InteropServices;
using Engine.Core;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace TriangleTest;

class TriangleTestEngine : EngineBase
{
    private Model<TriangleVertex> _triangleModel;
    private Model<TriangleVertex> _axisModel;

    public TriangleTestEngine()
        : base()
    { 
        Size = new(800, 600);
        Title = "Terrain Generator";
        ClearColor = new(0.2f, 0.3f, 0.3f);

        // Shader
        // TODO use relative pahts (maybe copy assets to bin folder?)
        var shader = new Shader
        (
            @"C:\Projects\OpenGL-Stuff\Test\TriangleTest\Assets\Shaders\triangle.vert",
            @"C:\Projects\OpenGL-Stuff\Test\TriangleTest\Assets\Shaders\triangle.frag"
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
        _camera.Position = new(10.0f, 10.0f, 10.0f);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        CursorState = CursorState.Grabbed;
        VSync = VSyncMode.On;
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (!IsFocused) return;

        const float moveSpeed = 10.0f;
        const float cameraSensitivity = 0.1f;

        var moveDir = Vector3.Zero;
        if (KeyboardState.IsKeyDown(Keys.W)) moveDir += _camera.Front;
        if (KeyboardState.IsKeyDown(Keys.S)) moveDir -= _camera.Front;
        if (KeyboardState.IsKeyDown(Keys.A)) moveDir -= _camera.Right;
        if (KeyboardState.IsKeyDown(Keys.D)) moveDir += _camera.Right;
        if (KeyboardState.IsKeyDown(Keys.Space)) moveDir += Vector3.UnitY;
        if (KeyboardState.IsKeyDown(Keys.LeftControl)) moveDir -= Vector3.UnitY;

        if (moveDir != Vector3.Zero)
        {
            _camera.Position += moveDir.Normalized() * moveSpeed * (float)args.Time;
        }

        if (MouseOffset != Vector2.Zero)
        {
            _camera.Yaw += MouseOffset.X * cameraSensitivity;
            _camera.Pitch -= MouseOffset.Y * cameraSensitivity;
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        _axisModel!.Render(UniformAccessor);
        _triangleModel!.Render(UniformAccessor);

        SwapBuffers();
    }
}
