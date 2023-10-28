using OpenTK.Windowing.Common;
using Engine;
using Engine.Graphics;

namespace TerrainGenerator;

class TerrainEngine : EngineBase
{
    private readonly Model _triangle;
    private readonly Shader _shader;

    public TerrainEngine()
        : base()
    { 
        Size = new(800, 600);
        Title = "Terrain Generator";
        ClearColor = new(0.2f, 0.3f, 0.3f);

        _triangle = new Model(new float[]
        {
            -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
             0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f,
             0.0f,  0.5f, 0.0f, 0.0f, 0.0f, 1.0f
        });

        _shader = new Shader
        (
            @"C:\Projects\OpenGL-Stuff\TerrainGenerator\Assets\Shaders\triangle.vert",
            @"C:\Projects\OpenGL-Stuff\TerrainGenerator\Assets\Shaders\triangle.frag"
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
