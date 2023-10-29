using OpenTK.Windowing.Common;
using Engine;
using Engine.Graphics;
using OpenTK.Graphics.OpenGL4;
using TerrainGenerator;
using System.Runtime.InteropServices;

namespace TriangleTest;

class TriangleTestEngine : EngineBase
{
    private readonly Model<TriangleVertex> _triangle;
    private readonly Shader _shader;

    public TriangleTestEngine()
        : base()
    { 
        Size = new(800, 600);
        Title = "Terrain Generator";
        ClearColor = new(0.2f, 0.3f, 0.3f);

        var vertices = new TriangleVertex[]
        {
            new TriangleVertex{ Position = new( 0.5f, -0.5f, 0.0f), Color = new(1.0f, 0.0f, 0.0f) },
            new TriangleVertex{ Position = new(-0.5f, -0.5f, 0.0f), Color = new(0.0f, 1.0f, 0.0f) },
            new TriangleVertex{ Position = new( 0.0f,  0.5f, 0.0f), Color = new(0.0f, 0.0f, 1.0f) }
        };

        _triangle = new Model<TriangleVertex>(vertices, vertices.Length, TriangleVertex.GetLayout(), BufferUsageHint.StaticDraw);

        // TODO use relative pahts (maybe copy assets to bin folder?)
        _shader = new Shader
        (
            @"C:\Projects\OpenGL-Stuff\Test\TriangleTest\Assets\Shaders\triangle.vert",
            @"C:\Projects\OpenGL-Stuff\Test\TriangleTest\Assets\Shaders\triangle.frag"
        );
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        _shader.Use();
        _triangle.Render();

        SwapBuffers();
    }
}
