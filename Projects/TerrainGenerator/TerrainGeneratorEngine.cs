using Engine.Core;
using Engine.Core.Controllers;
using Engine.Graphics;
using OpenTK.Windowing.Common;
using TerrainGenerator.Services;
using TerrainGenerator.Vertices;

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
        ((DefaultPlayerController)PlayerController).MovementSpeed = 40f;
        ((DefaultPlayerController)PlayerController).RunMultiplier = 2.5f;

        // Terrain chunk
        var terrainShader = Renderer.CreateShader
        (
            vertPath: "Assets/Shaders/terrain.vert",
            tescPath: "Assets/Shaders/terrain.tesc",
            tesePath: "Assets/Shaders/terrain.tese",
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

        var terrainMesh = Renderer.CreateMesh(terrainVertices, terrainIndices, TerrainVertex.GetLayout(), new MeshParameters { PrimitiveType = PrimitiveType.Patches });

        // Terrain Manager
        _terrainManager = new(Renderer, terrainMesh, terrainShader);

        // Camera
        Renderer.Camera.Position = new
        (
            _terrainManager.ChunkLength * 0.5f,
            _terrainManager.ChunkHeight * 1.5f,
            _terrainManager.ChunkLength * 0.5f
        );
    }

    private readonly TerrainManager _terrainManager;

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _terrainManager.Update();
    }

    protected override void OnRenderFrameInternal(FrameEventArgs args)
    {
        base.OnRenderFrameInternal(args);

        _terrainManager.Render(UniformManager);

        _terrainManager.RenderOverlay();
    }
}
