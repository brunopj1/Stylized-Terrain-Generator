using Engine.Core;
using Engine.Core.Controllers;
using Engine.Graphics;
using OpenTK.Windowing.Common;
using TerrainGenerator.Services;
using TerrainGenerator.Vertices;

// TODO tesselation shader not working
// TODO rendering quads not working

namespace TerrainGenerator;
internal class TerrainGeneratorEngine : AEngineBase
{
    public TerrainGeneratorEngine()
        : base()
    {
        Size = new(1600, 900);
        ClientLocation = new(50, 50);
        Title = "Terrain Generator";
        ClearColor = new(0.2f, 0.3f, 0.3f);

        // Player Controller
        PlayerController = new DefaultPlayerController(this);

        // Axis
        var axisShader = Renderer.CreateShader
        (
            vertPath: "Assets/Shaders/axis.vert",
            fragPath: "Assets/Shaders/axis.frag"
        );

        var axisVertices = new AxisVertex[]
        {
            new AxisVertex{ Position = new( 0f, 0f, 0f), Color = new(1f, 0f, 0f) },
            new AxisVertex{ Position = new( 1f, 0f, 0f), Color = new(1f, 0f, 0f) },
            new AxisVertex{ Position = new( 0f, 0f, 0f), Color = new(0f, 1f, 0f) },
            new AxisVertex{ Position = new( 0f, 1f, 0f), Color = new(0f, 1f, 0f) },
            new AxisVertex{ Position = new( 0f, 0f, 0f), Color = new(0f, 0f, 1f) },
            new AxisVertex{ Position = new( 0f, 0f, 1f), Color = new(0f, 0f, 1f) }
        };

        var axisMesh = Renderer.CreateMesh(axisVertices, AxisVertex.GetLayout(), new() { PrimitiveType = PrimitiveType.Lines });

        _axisModel = Renderer.CreateModel(axisMesh, axisShader);
        _axisModel.Transform.ScaleBy(new Vector3(30));

        // Terrain chunk
        var terrainShader = Renderer.CreateShader
        (
            vertPath: "Assets/Shaders/terrain.vert",
            fragPath: "Assets/Shaders/terrain.frag"
        );

        var terrainVertices = new TerrainVertex[]
        {
            new TerrainVertex{ Position = new(1, 1) },
            new TerrainVertex{ Position = new(1, 0) },
            new TerrainVertex{ Position = new(0, 1) },
            new TerrainVertex{ Position = new(0, 0) }
        };
        var terrainIndices = new uint[] { 0, 1, 2, 2, 1, 3 };

        var terrainMesh = Renderer.CreateMesh(terrainVertices, terrainIndices, TerrainVertex.GetLayout(), new MeshParameters { PrimitiveType = PrimitiveType.Triangles });

        // Terrain Manager
        _terrainManager = new(Renderer, terrainMesh, terrainShader);

        // Camera
        Renderer.Camera.Position = new
        (
            _terrainManager.ChunkLength * 0.5f,
            _terrainManager.ChunkHeight + 10,
            _terrainManager.ChunkLength * 0.5f
        );
        Renderer.Camera.Near = 1f;
        Renderer.Camera.Far = 1000f;
    }

    private readonly TerrainManager _terrainManager;
    private readonly Model _axisModel;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _terrainManager.Update();
    }

    protected override void OnRenderFrameInternal(FrameEventArgs args)
    {
        base.OnRenderFrameInternal(args);

        _axisModel.Render(Renderer.Camera, UniformManager);

        _terrainManager.Render(UniformManager);
    }
}
