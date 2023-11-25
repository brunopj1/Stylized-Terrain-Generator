using Engine.Core;
using Engine.Graphics;
using Engine.Util.PlayerControllers;
using Engine.Util.SmartProperties.Properties;
using OpenTK.Windowing.Common;
using TerrainGenerator.Graphics;
using TerrainGenerator.Services;

namespace TerrainGenerator;
internal class TerrainGeneratorEngine : AEngineBase
{
    public TerrainGeneratorEngine()
        : base()
    {
        Size = new(1600, 900);
        ClientLocation = new(50, 50);
        Title = "Terrain Generator";
        ClearColor = null;
        VSync = VSyncMode.On;

        // Player Controller
        PlayerController = new FlyingPlayerController(this);
        ((FlyingPlayerController)PlayerController).MovementSpeed = 150f;
        ((FlyingPlayerController)PlayerController).RunMultiplier = 5f;

        // Services
        _terrainManager = new(Renderer, ImGuiRenderer);
        _skyManager = new(Renderer, ImGuiRenderer);

        // Camera
        Renderer.Camera.Position = new
        (
            _terrainManager.ChunkLength.Value * 0.5f,
            _terrainManager.ChunkHeight.Value * 1.0f,
            _terrainManager.ChunkLength.Value * 0.5f
        );
    }

    private readonly TerrainManager _terrainManager;
    private readonly SkyManager _skyManager;

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ProvokingVertex(ProvokingVertexMode.FirstVertexConvention);

        _terrainManager.Load();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _terrainManager.Update();
    }

    protected override void OnRecompileShaders()
    {
        _terrainManager.UpdateChunkTextures();
    }
}
