using System.ComponentModel.DataAnnotations;
using Engine.Core.Services;
using Engine.Graphics;
using Engine.Helpers;
using Engine.Util.SmartProperties;
using Engine.Util.SmartProperties.Properties;
using ImGuiNET;
using TerrainGenerator.Entities;
using TerrainGenerator.Graphics;

namespace TerrainGenerator.Services;

// TODO connections between LODs
// TODO fix: modifying the length seems to modify the height as well
internal class TerrainManager : ICustomUniformManager
{
    public TerrainManager(Renderer renderer, ImGuiRenderer imGuiRenderer)
    {
        _renderer = renderer;

        // Shaders

        _renderShader = renderer.CreateRenderShader
        (
            vertPath: "Assets/Shaders/terrain.vert",
            fragPath: "Assets/Shaders/terrain.frag"
        );

        _computeShader = renderer.CreateComputeShader("Assets/Shaders/terrain.comp");

        // Tessellation

        _tessellationMap = new();
        _tessellationMap.Add(new(4, 64, this));
        _tessellationMap.Add(new(4, 32, this));
        _tessellationMap.Add(new(4, 16, this));
        _tessellationMap.Add(new(4, 8, this));
        _tessellationMap.Add(new(4, 4, this));

        // Smart Properties

        _gridPropertyGroup = new("Grid Settings");
        imGuiRenderer.AddOverlay(_gridPropertyGroup.RenderWindow);

        DynamicTerrainOffset = new(_gridPropertyGroup, "Dynamic Terrain Offset", true);

        GridOffset = new(_gridPropertyGroup, "Grid Offset", Vector2i.Zero);
        GridOffset.OnValueModified += (oldValue, newValue) =>
        {
            var offset = newValue - oldValue;
            UpdateChunkOffsets();
            OffsetChunkTextures(offset);
        };

        ChunkLength = new(_gridPropertyGroup, "Chunk Length", 500f)
        {
            Range = new() { Min = 10, Max = 1000 },
            RenderSettings = new() { DragStep = 2f }
        };
        ChunkLength.OnValueModified += (oldValue, newValue) =>
        {
            UpdateChunkTransformsAndBVs();
            UpdateChunkTextures();
            UpdateCameraViewDistance();
        };

        ChunkHeight = new(_gridPropertyGroup, "Chunk Height", 3500f)
        {
            Range = new() { Min = 0, Max = 10000 },
            RenderSettings = new() { DragStep = 10f }
        };
        ChunkHeight.OnValueModified += (oldValue, newValue) =>
        {
            UpdateChunkTransformsAndBVs();
            UpdateChunkTextures();
        };

        ChunkHeightStep = new(_gridPropertyGroup, "Chunk Height Step", 3f)
        {
            Range = new() { Min = 0.1f, Max = 100 },
            RenderSettings = new() { DragStep = 0.1f }
        };
        ChunkHeightStep.OnValueModified += (oldValue, newValue) =>
        {
            UpdateChunkTextures();
        };
    }

    private readonly Renderer _renderer;

    private readonly RenderShader _renderShader;
    private readonly ComputeShader _computeShader;

    private Chunk[,] _chunkGrid = new Chunk[0, 0];
    private uint _chunkRadius = 0;
    private Chunk? _currentChunk = null;

    private readonly PropertyGroup _gridPropertyGroup;

    public BoolProperty DynamicTerrainOffset { get; }
    public Vector2iProperty GridOffset { get; }
    public FloatProperty ChunkLength { get; }
    public FloatProperty ChunkHeight { get; }
    public FloatProperty ChunkHeightStep { get; }

    private readonly TesselationMap _tessellationMap;

    public void Load()
    {
        UpdateChunkGrid();
        UpdateCameraViewDistance();
    }

    public void Update()
    {
        var cameraPos = _renderer.Camera.Position;
        var chunkLength = ChunkLength.Value;

        var offsetX = (int)MathF.Floor(cameraPos.X / chunkLength);
        var offsetZ = (int)MathF.Floor(cameraPos.Z / chunkLength);

        if (DynamicTerrainOffset.Value && (offsetX != 0 || offsetZ != 0))
        {
            Vector2i cameraOffset = new Vector2i(offsetX, offsetZ);
            _renderer.Camera.Position -= new Vector3(offsetX * chunkLength, 0, offsetZ * chunkLength);
            GridOffset.Value += cameraOffset;
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
                var chunk = new Chunk(this);
                chunk.Model = _renderer.CreateModel(null, _renderShader);
                chunk.Model.CustomUniformManager = chunk;

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
        var chunkLength = ChunkLength.Value;

        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                var model = _chunkGrid[i, j].Model;

                model.Transform.Position = new((i - _chunkRadius) * chunkLength, 0, (j - _chunkRadius) * chunkLength);
                model.Transform.Scale = new(chunkLength, 1, chunkLength);

                model.BoundingVolume = new AxisAlignedBoundingBox(Vector3.Zero, new Vector3(1, ChunkHeight.Value, 1));
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
        var gridOffset = GridOffset.Value;

        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                var chunk = _chunkGrid[i, j];

                var localOffset = new Vector2i(i - (int)_chunkRadius, j - (int)_chunkRadius);
                chunk.Offset = gridOffset + localOffset;
            }
        }
    }

    public void UpdateChunkTextures()
    {
        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                var chunk = _chunkGrid[i, j];

                if (chunk.Texture == null) CreateChunkTexture(chunk);
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
                    var temp = chunk.Texture;
                    chunk.Texture = otherChunk.Texture;
                    otherChunk.Texture = temp;
                }
            }
        }
    }

    private void UpdateCameraViewDistance()
    {
        _renderer.Camera.Far = MathF.Max((_chunkRadius + 1) * 1.3f * ChunkLength.Value, 1000);
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
            _renderer.DestroyTexture(chunk.Texture);
            chunk.Texture = null;
        }
    }

    public void BindUniforms(AShader shader)
    {
        _gridPropertyGroup.BindUniforms(shader);

        shader.BindUniform("uMaxChunkDivisions", _tessellationMap.MaxDivisions);
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

            temp = (int)zone.Divisions;
            if (ImGuiHelper.DragIntClamped("Divisions", ref temp, 1, 1, 256))
            {
                zone.Divisions = (uint)temp;
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

    private void CreateChunkTexture(Chunk chunk)
    {
        var textureSize = new Vector2i((int)chunk.Divisions + 1);

        var parameters = new TextureParameters
        {
            PixelInternalFormat = PixelInternalFormat.Rgba32ui,
            PixelFormat = PixelFormat.RgInteger,
            PixelType = PixelType.UnsignedInt,

            MinFilter = TextureMinFilter.Nearest,
            MagFilter = TextureMagFilter.Nearest,
            WrapModeS = TextureWrapMode.ClampToEdge,
            WrapModeT = TextureWrapMode.ClampToEdge
        };

        chunk.Texture = _renderer.CreateTexture(textureSize, parameters);
    }

    private void ComputeChunkTextures(Chunk chunk)
    {
        _currentChunk = chunk;

        _computeShader.Use();

        chunk.BindUniforms(_computeShader);

        _computeShader.BindUniform("uChunkTexture", chunk.Texture, 0, TextureAccess.WriteOnly);

        var size = new Vector2i((int)chunk.Divisions + 1);
        _computeShader.Dispatch(size.X, size.Y, 1);

        _currentChunk = null;
    }
}
