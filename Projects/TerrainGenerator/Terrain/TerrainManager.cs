using System.ComponentModel.DataAnnotations;
using Engine.Core.Services;
using Engine.Graphics;
using Engine.Helpers;
using ImGuiNET;
using TerrainGenerator.Graphics;
using TerrainGenerator.Terrain;
using TerrainGenerator.Terrain.Entities;

namespace TerrainGenerator.Services;

// TODO connections between LODs
// TODO fix weird lines in the terrain color (easy to notice when changing chunk length)
internal class TerrainManager : ICustomUniformManager
{
    public TerrainManager(Renderer renderer)
    {
        _renderer = renderer;

        _renderShader = renderer.CreateRenderShader
        (
            vertPath: "Assets/Shaders/terrain.vert",
            fragPath: "Assets/Shaders/terrain.frag"
        );

        _computeShader = renderer.CreateComputeShader("Assets/Shaders/terrain.comp");

        _tessellationMap = new();
        _tessellationMap.Add(new(4, 5, this));
        _tessellationMap.Add(new(4, 4, this));
        _tessellationMap.Add(new(4, 3, this));
        _tessellationMap.Add(new(4, 2, this));
        _tessellationMap.Add(new(4, 1, this));

        // Fake initialization
        _chunkGrid = new Chunk[0, 0];
        _gridOffset = Vector2i.Zero;
        _currentChunk = null;
    }

    private readonly Renderer _renderer;

    private readonly RenderShader _renderShader;
    private readonly ComputeShader _computeShader;

    private Chunk[,] _chunkGrid;
    private uint _chunkRadius;
    private Vector2i _gridOffset;

    private Chunk? _currentChunk;

    private float _chunkLength = 500;
    public float ChunkLength
    {
        get => _chunkLength;
        set
        {
            _chunkLength = value;
            UpdateChunkTransformsAndBVs();
            UpdateChunkTextures();
            UpdateCameraViewDistance();
        }
    }

    private float _chunkHeight = 3500;
    public float ChunkHeight
    {
        get => _chunkHeight;
        set
        {
            _chunkHeight = value;
            UpdateChunkTransformsAndBVs();
        }
    }

    private readonly TesselationMap _tessellationMap;

    public void Load()
    {
        UpdateChunkGrid();
        UpdateCameraViewDistance();
    }

    public void Update()
    {
        var cameraPos = _renderer.Camera.Position;

        var offsetX = (int)MathF.Floor(cameraPos.X / _chunkLength);
        var offsetZ = (int)MathF.Floor(cameraPos.Z / _chunkLength);

        if (offsetX != 0 || offsetZ != 0)
        {
            Vector2i cameraOffset = new Vector2i(offsetX, offsetZ);
            _gridOffset += cameraOffset;
            _renderer.Camera.Position -= new Vector3(offsetX * _chunkLength, 0, offsetZ * _chunkLength);

            UpdateChunkOffsets();
            OffsetChunkTextures(cameraOffset);
        }
    }

    private void UpdateChunkGrid()
    {
        DestroyAllChunkModels();
        DestroyAllChunkTextures();

        _chunkRadius = _tessellationMap.TotalRadius;
        var gridSize = _chunkRadius * 2 + 1;
        _chunkGrid = new Chunk[gridSize, gridSize];

        for (var i = 0; i < gridSize; i++)
        {
            for (var j = 0; j < gridSize; j++)
            {
                var chunk = new Chunk();
                chunk.Model = _renderer.CreateModel(null, _renderShader, customUniformManager: this);

                _chunkGrid[i, j] = chunk;
            }
        }

        UpdateChunkMeshesAndTessellations();
        UpdateChunkTransformsAndBVs();
        UpdateChunkOffsets();
        UpdateChunkTextures();
    }

    private void UpdateChunkTransformsAndBVs()
    {
        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                var model = _chunkGrid[i, j].Model;

                model.Transform.Position = new((i - _chunkRadius) * _chunkLength, 0, (j - _chunkRadius) * _chunkLength);
                model.Transform.Scale = new(_chunkLength, 1, _chunkLength);

                model.BoundingVolume = new AxisAlignedBoundingBox(Vector3.Zero, new Vector3(1, _chunkHeight, 1));
            }
        }
    }

    private void UpdateChunkMeshesAndTessellations()
    {
        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                var chunk = _chunkGrid[i, j];

                var localOffset = new Vector2i(i - (int)_chunkRadius, j - (int)_chunkRadius);
                var centerDistance = (uint)MathF.Max(MathF.Abs(localOffset.X), MathF.Abs(localOffset.Y));
                var zone = _tessellationMap.GetCorrespondingZone(localOffset);

                chunk.Model.Mesh = zone.Mesh;
                chunk.Divisions = zone.Divisions;
            }
        }
    }

    private void UpdateChunkOffsets()
    {
        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                var chunk = _chunkGrid[i, j];

                var localOffset = new Vector2i(i - (int)_chunkRadius, j - (int)_chunkRadius);
                chunk.Offset = _gridOffset + localOffset;
            }
        }
    }

    private void UpdateChunkTextures()
    {
        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                var chunk = _chunkGrid[i, j];

                if (chunk.HightmapTexture == null) CreateChunkTextures(chunk);
                ComputeChunkTextures(chunk);
            }
        }
    }

    private void OffsetChunkTextures(Vector2i localOffset)
    {
        int startI = 0, endI = _chunkGrid.GetLength(0), deltaI = 1;
        int startJ = 0, endJ = _chunkGrid.GetLength(1), deltaJ = 1;

        if (localOffset.X < 0)
        {
            startI = endI - 1;
            endI = -1;
            deltaI = -1;
        }
        else if (localOffset.Y < 0)
        {
            startJ = endJ - 1;
            endJ = -1;
            deltaJ = -1;
        }

        for (var i = startI; i != endI; i += deltaI)
        {
            for (var j = startJ; j != endJ; j += deltaJ)
            {
                var chunk = _chunkGrid[i, j];
                var otherChunk = GetChunk(i + localOffset.X, j + localOffset.Y);

                if (otherChunk == null || chunk.Divisions != otherChunk.Divisions)
                {
                    ComputeChunkTextures(chunk);
                }
                else
                {
                    var temp = (chunk.HightmapTexture, chunk.ColormapTexture);

                    chunk.HightmapTexture = otherChunk.HightmapTexture;
                    chunk.ColormapTexture = otherChunk.ColormapTexture;

                    otherChunk.HightmapTexture = temp.Item1;
                    otherChunk.ColormapTexture = temp.Item2;
                }
            }
        }
    }

    private void UpdateCameraViewDistance()
    {
        _renderer.Camera.Far = MathF.Max((_chunkRadius + 1) * 1.3f * _chunkLength, 1000);
        _renderer.Camera.Near = _renderer.Camera.Far * 0.001f;
    }

    private Chunk? GetChunk(int i, int j)
    {
        if (i < 0 || i >= _chunkGrid.GetLength(0) || j < 0 || j >= _chunkGrid.GetLength(1)) return null;
        return _chunkGrid[i, j];
    }

    private void DestroyAllChunkModels()
    {
        foreach (var chunk in _chunkGrid)
        {
            _renderer.DestroyModel(chunk.Model);
            chunk.Model = null;
        }
    }

    private void DestroyAllChunkTextures()
    {
        foreach (var chunk in _chunkGrid)
        {
            _renderer.DestroyTexture(chunk.HightmapTexture);
            chunk.HightmapTexture = null;

            _renderer.DestroyTexture(chunk.ColormapTexture);
            chunk.ColormapTexture = null;

        }
    }

    public void BindUniforms(AShader shader)
    {
        shader.BindUniform("uChunkLength", _chunkLength);
        shader.BindUniform("uChunkHeight", _chunkHeight);
        shader.BindUniform("uMaxChunkDivisions", _tessellationMap.MaxDivisions);

        if (_currentChunk != null)
        {
            shader.BindUniform("uChunkOffset", _currentChunk.Offset);
            shader.BindUniform("uChunkDivisions", _currentChunk.Divisions);
            shader.BindUniform("uChunkHeightmap", _currentChunk.HightmapTexture, 0);
            shader.BindUniform("uChunkColormap", _currentChunk.ColormapTexture, 1);
        }
    }

    public long RenderTerrain(EngineUniformManager uniformManager)
    {
        var count = 0L;

        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                _currentChunk = _chunkGrid[i, j];
                count += _currentChunk.Model.Render(_renderer.Camera, uniformManager);
            }
        }

        _currentChunk = null;

        return count;
    }

    public void RenderOverlay()
    {
        RenderTerrainSettingsWindow();
        RenderTessellationSettingsWindow();
    }

    private void RenderTerrainSettingsWindow()
    {
        ImGui.Begin("Grid Settings");

        var temp = _chunkLength;
        if (ImGuiHelper.DragFloatClamped("Chunk length", ref temp, 2, 10, 1000)) ChunkLength = temp;

        temp = _chunkHeight;
        if (ImGuiHelper.DragFloatClamped("Chunk height", ref temp, 8, 0, 10000)) ChunkHeight = temp;

        ImGui.End();
    }

    public void RenderTessellationSettingsWindow()
    {
        ImGui.Begin("Tessellation Settings");

        for (var i = 0; i < _tessellationMap.Count; i++)
        {
            var zone = _tessellationMap[i];

            ImGui.PushID(i);

            var temp = (int)zone.Distance;
            if (ImGuiHelper.InputIntClamped("Distance", ref temp, 1, 1, 50))
            {
                zone.Distance = (uint)temp;
                UpdateChunkGrid();
                UpdateCameraViewDistance();
            }

            temp = (int)zone.DivisionsLog2;
            if (ImGuiHelper.InputIntClamped("Divisions", ref temp, 1, 0, 8))
            {
                zone.DivisionsLog2 = (uint)temp;
                _renderer.DestroyMesh(zone.Mesh);
                zone.Mesh = CreateChunkMesh(zone.Divisions);
                UpdateChunkMeshesAndTessellations();
                DestroyAllChunkTextures();
                UpdateChunkTextures();
            }

            if (ImGui.Button(@"/\") && i != 0)
            {
                var tempZone = _tessellationMap[i - 1];
                _tessellationMap[i - 1] = zone;
                _tessellationMap[i] = tempZone;
                UpdateChunkMeshesAndTessellations();
                DestroyAllChunkTextures();
                UpdateChunkTextures();
            }

            ImGui.SameLine();

            if (ImGui.Button(@"\/") && i != _tessellationMap.Count - 1)
            {
                var tempZone = _tessellationMap[i + 1];
                _tessellationMap[i + 1] = zone;
                _tessellationMap[i] = tempZone;
                UpdateChunkMeshesAndTessellations();
                DestroyAllChunkTextures();
                UpdateChunkTextures();
            }

            ImGui.SameLine();

            if (ImGui.Button("X") && _tessellationMap.Count > 1)
            {
                _tessellationMap.RemoveAt(i);
                _renderer.DestroyMesh(zone.Mesh);
                UpdateChunkGrid();
            }

            ImGui.PopID();

            ImGui.Separator();
            ImGui.Separator();
        }

        if (ImGui.Button("+"))
        {
            _tessellationMap.Add(new(1, 0, this));
            UpdateChunkGrid();
        }

        ImGui.End();
    }

    public Mesh CreateChunkMesh(uint divisions)
    {
        var capacity = divisions * divisions * 2 * 3;
        var vertices = new TerrainVertex[capacity];

        var divisionOffset = 1f / divisions;

        var idx = 0;
        for (var i = 0; i < divisions; i++)
        {
            for (var j = 0; j < divisions; j++)
            {
                // Triangle 0
                var pos = new Vector2(i, j) * divisionOffset;

                vertices[idx++] = new(pos, 0f);
                vertices[idx++] = new(pos + new Vector2(0, divisionOffset), 0f);
                vertices[idx++] = new(pos + new Vector2(divisionOffset, 0), 0f);

                // Triangle 1
                pos = new Vector2(i + 1, j + 1) * divisionOffset;

                vertices[idx++] = new(pos, 1f);
                vertices[idx++] = new(pos - new Vector2(0, divisionOffset), 1f);
                vertices[idx++] = new(pos - new Vector2(divisionOffset, 0), 1f);
            }
        }

        return _renderer.CreateMesh(vertices, TerrainVertex.GetLayout());
    }

    private void CreateChunkTextures(Chunk chunk)
    {
        var heightmapSize = new Vector2i((int)chunk.Divisions + 1);
        var colormapSize = new Vector2i((int)chunk.Divisions) * new Vector2i(2, 1);

        var parameters = new TextureParameters
        {
            MinFilter = TextureMinFilter.Nearest,
            MagFilter = TextureMagFilter.Nearest,
            WrapModeS = TextureWrapMode.ClampToEdge,
            WrapModeT = TextureWrapMode.ClampToEdge
        };

        chunk.HightmapTexture = _renderer.CreateTexture(heightmapSize, parameters);
        chunk.ColormapTexture = _renderer.CreateTexture(colormapSize, parameters);
    }

    private void ComputeChunkTextures(Chunk chunk)
    {
        _currentChunk = chunk;

        _computeShader.Use();

        BindUniforms(_computeShader);
        // TODO try to optimize this to use only one texture
        _computeShader.BindUniform("uChunkHeightmap", chunk.HightmapTexture, 0, TextureAccess.WriteOnly);
        _computeShader.BindUniform("uChunkColormap", chunk.ColormapTexture, 1, TextureAccess.WriteOnly);

        var size = new Vector2i((int)chunk.Divisions + 1);
        _computeShader.Dispatch(size.X, size.Y, 1);

        _currentChunk = null;
    }
}
