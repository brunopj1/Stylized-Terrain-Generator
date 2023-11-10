using Engine.Core;
using Engine.Core.Controllers;
using Engine.Graphics;
using OpenTK.Windowing.Common;
using TerrainGenerator.Graphics;
using TerrainGenerator.Services;
using TerrainGenerator.Terrain;

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
        ((DefaultPlayerController)PlayerController).MovementSpeed = 150f;
        ((DefaultPlayerController)PlayerController).RunMultiplier = 5f;

        // Terrain Manager
        _terrainManager = new(Renderer);

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
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _terrainManager.Update();
    }

    protected override void OnRenderFrameInternal(FrameEventArgs args)
    {
        base.OnRenderFrameInternal(args);

        _terrainManager.RenderTerrain(UniformManager);

        _terrainManager.RenderOverlay();
    }
}
