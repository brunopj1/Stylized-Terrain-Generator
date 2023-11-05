using Engine.Core.Services;
using Engine.Graphics;

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
    }

    private readonly Renderer _renderer;
    private readonly Mesh _mesh;
    private readonly Shader _shader;

    private Model[,] _chunkGrid;
    private Vector2i _gridOffset;
    private Vector2i _chunkOffset; // For usage in the shader

    private uint _chunkRadius = 8;
    public uint ChunkRadius
    {
        get => _chunkRadius;
        set
        {
            _chunkRadius = value;
            UpdateChunkGrid();
        }
    }

    private float _chunkLength = 50;
    public float ChunkLength
    {
        get => _chunkLength;
        set
        {
            _chunkLength = value;
            UpdateChunks();
        }
    }

    private float _chunkHeight = 100;
    public float ChunkHeight
    {
        get => _chunkHeight;
        set => _chunkHeight = value;
    }

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
                _chunkGrid[i, j].BoundingVolume = new AxisAlignedBoundingBox(Vector3.Zero, new Vector3(1, _chunkHeight, 1));
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
            }
        }
    }

    public void BindUniforms(Shader shader)
    {
        shader.BindUniform("uChunkLength", _chunkLength);
        shader.BindUniform("uChunkHeight", _chunkHeight);
        shader.BindUniform("uChunkOffset", ref _chunkOffset);
    }
}
