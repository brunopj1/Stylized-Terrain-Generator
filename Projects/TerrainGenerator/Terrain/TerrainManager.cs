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
// TODO chunk textures -> create once, then modify the same texture
// TODO chunk textures -> move textures between chunks on offset change
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
            _gridOffset += new Vector2i(offsetX, offsetZ);
            _renderer.Camera.Position -= new Vector3(offsetX * _chunkLength, 0, offsetZ * _chunkLength);

            UpdateChunkOffsets();
            UpdateChunkTextures();
        }
    }

    private void UpdateChunkGrid()
    {
        foreach (var chunk in _chunkGrid)
        {
            _renderer.DestroyMesh(chunk.Model.Mesh);
            if (chunk.HightmapTexture != null)
            {
                _renderer.DestroyTexture(chunk.HightmapTexture);
                _renderer.DestroyTexture(chunk.ColormapTexture);
            }
        }

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
                _currentChunk = _chunkGrid[i, j];

                if (_currentChunk.HightmapTexture != null)
                {
                    _renderer.DestroyTexture(_currentChunk.HightmapTexture);
                    _renderer.DestroyTexture(_currentChunk.ColormapTexture);
                }

                CreateChunkTextures(_currentChunk);
            }
        }

        _currentChunk = null;
    }

    private void UpdateCameraViewDistance()
    {
        _renderer.Camera.Far = MathF.Max((_chunkRadius + 1) * 1.3f * _chunkLength, 1000);
        _renderer.Camera.Near = _renderer.Camera.Far * 0.001f;
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

    public void RenderTerrain(EngineUniformManager uniformManager)
    {
        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                _currentChunk = _chunkGrid[i, j];
                _currentChunk.Model.Render(_renderer.Camera, uniformManager);
            }
        }

        _currentChunk = null;
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
            if (ImGuiHelper.DragIntClamped("Distance", ref temp, 1, 1, 50))
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
                UpdateChunkTextures();
            }

            if (ImGui.Button(@"/\") && i != 0)
            {
                var tempZone = _tessellationMap[i - 1];
                _tessellationMap[i - 1] = zone;
                _tessellationMap[i] = tempZone;
                UpdateChunkMeshesAndTessellations();
                UpdateChunkTextures();
            }

            ImGui.SameLine();

            if (ImGui.Button(@"\/") && i != _tessellationMap.Count - 1)
            {
                var tempZone = _tessellationMap[i + 1];
                _tessellationMap[i + 1] = zone;
                _tessellationMap[i] = tempZone;
                UpdateChunkMeshesAndTessellations();
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

        _computeShader.Use();

        BindUniforms(_computeShader);
        // TODO try to optimize this to use only one texture
        _computeShader.BindUniform("uChunkHeightmap", chunk.HightmapTexture, 0, TextureAccess.WriteOnly);
        _computeShader.BindUniform("uChunkColormap", chunk.ColormapTexture, 1, TextureAccess.WriteOnly);

        _computeShader.Dispatch(heightmapSize.X, heightmapSize.Y, 1);
    }
}
