using Engine.Core.Services;
using Engine.Graphics;
using ImGuiNET;

namespace TerrainGenerator.Services;
internal class TerrainManager : ICustomUniformManager
{
    public TerrainManager(Renderer renderer, Mesh mesh, Shader shader)
    {
        _renderer = renderer;
        _mesh = mesh;
        _shader = shader;

        // Fake initialization
        _chunkGrid = new Model[0, 0];
        _gridOffset = new();
        _chunkOffset = new();

        UpdateChunkGrid();
        UpdateCameraViewDistance();
    }

    private readonly Renderer _renderer;
    private readonly Mesh _mesh;
    private readonly Shader _shader;

    private Model[,] _chunkGrid;
    private Vector2i _gridOffset;
    private Vector2i _chunkOffset; // For usage in the shader

    private uint _chunkRadius = 15;
    public uint ChunkRadius
    {
        get => _chunkRadius;
        set
        {
            _chunkRadius = value;
            UpdateChunkGrid();
            UpdateCameraViewDistance();
        }
    }

    private float _chunkLength = 200;
    public float ChunkLength
    {
        get => _chunkLength;
        set
        {
            _chunkLength = value;
            UpdateChunks();
            UpdateCameraViewDistance();
        }
    }

    private float _chunkHeight = 200;
    public float ChunkHeight
    {
        get => _chunkHeight;
        set
        {
            _chunkHeight = value;
            UpdateChunks();
        }
    }

    // TODO move this to a different class
    private float _terrainFrequency = 0.01f;

    public void Update()
    {
        var cameraPos = _renderer.Camera.Position;

        var offsetX = (int)MathF.Floor(cameraPos.X / _chunkLength);
        var offsetZ = (int)MathF.Floor(cameraPos.Z / _chunkLength);

        if (offsetX != 0 || offsetZ != 0)
        {
            _gridOffset += new Vector2i(offsetX, offsetZ);
            _renderer.Camera.Position -= new Vector3(offsetX * _chunkLength, 0, offsetZ * _chunkLength);
        }
    }

    private void UpdateChunkGrid()
    {
        foreach (var chunk in _chunkGrid)
        {

            _renderer.DestroyModel(chunk);
        }

        var size = _chunkRadius * 2 + 1;
        _chunkGrid = new Model[size, size];

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                _chunkGrid[i, j] = _renderer.CreateModel(_mesh, _shader, customUniformManager: this);
            }
        }

        UpdateChunks();
    }

    private void UpdateChunks()
    {
        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                var chunk = _chunkGrid[i, j];
                chunk.Transform.Position = new((i - _chunkRadius) * _chunkLength, 0, (j - _chunkRadius) * _chunkLength);
                chunk.Transform.Scale = new(_chunkLength, 1, _chunkLength);
                chunk.BoundingVolume = new AxisAlignedBoundingBox(Vector3.Zero, new Vector3(1, _chunkHeight, 1));
            }
        }
    }

    private void UpdateCameraViewDistance()
    {
        _renderer.Camera.Far = (_chunkRadius + 1) * 1.2f * _chunkLength;
        _renderer.Camera.Near = _renderer.Camera.Far * 0.001f;
    }

    public void Render(EngineUniformManager uniformManager)
    {
        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                _chunkOffset = _gridOffset + new Vector2i(i - (int)_chunkRadius, j - (int)_chunkRadius);
                _chunkGrid[i, j].Render(_renderer.Camera, uniformManager);
            }
        }
    }

    public void RenderOverlay()
    {
        ImGui.Begin("Terrain Grid Settings");

        var tempF = _chunkLength;
        if (ImGui.DragFloat("Chunk length", ref tempF, 1, 10, 500)) ChunkLength = tempF;

        tempF = _chunkHeight;
        if (ImGui.DragFloat("Chunk height", ref tempF, 1, 0, 1000)) ChunkHeight = tempF;

        var tempI = (int)_chunkRadius;
        if (ImGui.DragInt("Chunk radius", ref tempI, 1, 1, 50)) ChunkRadius = (uint)tempI;

        tempF = _terrainFrequency;
        if (ImGui.DragFloat("Terrain frequency", ref tempF, 0.0001f, 0.001f, 0.03f)) _terrainFrequency = tempF;

        ImGui.End();
    }

    public void BindUniforms(Shader shader)
    {
        shader.BindUniform("uChunkLength", _chunkLength);
        shader.BindUniform("uChunkHeight", _chunkHeight);
        shader.BindUniform("uChunkOffset", ref _chunkOffset);
        shader.BindUniform("uTerrainFrequency", _terrainFrequency);
    }
}
