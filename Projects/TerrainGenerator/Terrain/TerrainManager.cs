using System.ComponentModel.DataAnnotations;
using Engine.Core.Services;
using Engine.Graphics;
using Engine.Helpers;
using ImGuiNET;
using TerrainGenerator.Extensions;
using TerrainGenerator.Terrain;
using TerrainGenerator.Terrain.Entities;

namespace TerrainGenerator.Services;

// TODO connections betwwen LODs
internal class TerrainManager : ICustomUniformManager
{
    public TerrainManager(Renderer renderer)
    {
        _renderer = renderer;

        _shader = renderer.CreateShader
        (
            vertPath: "Assets/Shaders/terrain.vert",
            fragPath: "Assets/Shaders/terrain.frag"
        );

        _tessellationZones = new()
        {
            new(4, 5, renderer),
            new(4, 4, renderer),
            new(4, 3, renderer),
            new(4, 2, renderer),
            new(4, 1, renderer)
        };

        // Fake initialization
        _chunkGrid = new Chunk[0, 0];
        _currentChunk = null;
        _gridOffset = new();

        UpdateChunkGrid();
        UpdateCameraViewDistance();
    }

    private readonly Renderer _renderer;

    private readonly Shader _shader;

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

    private readonly List<TesselationZone> _tessellationZones;

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
        }
    }

    private void UpdateChunkGrid()
    {
        foreach (var chunk in _chunkGrid)
        {
            _renderer.DestroyModel(chunk.Model);
        }

        _chunkRadius = (uint)_tessellationZones.Sum(zone => zone.Distance);
        var gridSize = _chunkRadius * 2 + 1;
        _chunkGrid = new Chunk[gridSize, gridSize];

        for (var i = 0; i < gridSize; i++)
        {
            for (var j = 0; j < gridSize; j++)
            {
                var model = _renderer.CreateModel(null, _shader, customUniformManager: this);
                _chunkGrid[i, j] = new(model);
            }
        }

        UpdateChunkMeshesAndTessellations();
        UpdateChunkTransformsAndBVs();
        UpdateChunkOffsets();
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
                var zone = _tessellationZones.GetCorrespondingZone(localOffset);

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

    private void UpdateCameraViewDistance()
    {
        _renderer.Camera.Far = MathF.Max((_chunkRadius + 1) * 1.3f * _chunkLength, 1000);
        _renderer.Camera.Near = _renderer.Camera.Far * 0.001f;
    }

    public void BindUniforms(Shader shader)
    {
        shader.BindUniform("uChunkLength", _chunkLength);
        shader.BindUniform("uChunkHeight", _chunkHeight);

        shader.BindUniform("uChunkOffset", _currentChunk!.Offset);
        shader.BindUniform("uChunkDivisions", _currentChunk!.Divisions);
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

        for (var i = 0; i < _tessellationZones.Count; i++)
        {
            var zone = _tessellationZones[i];

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
                zone.UpdateMesh(_renderer);
                UpdateChunkMeshesAndTessellations();
            }

            if (ImGui.Button(@"/\") && i != 0)
            {
                var tempZone = _tessellationZones[i - 1];
                _tessellationZones[i - 1] = zone;
                _tessellationZones[i] = tempZone;
                UpdateChunkMeshesAndTessellations();
            }

            ImGui.SameLine();

            if (ImGui.Button(@"\/") && i != _tessellationZones.Count - 1)
            {
                var tempZone = _tessellationZones[i + 1];
                _tessellationZones[i + 1] = zone;
                _tessellationZones[i] = tempZone;
                UpdateChunkMeshesAndTessellations();
            }

            ImGui.SameLine();

            if (ImGui.Button("X") && _tessellationZones.Count > 1)
            {
                _tessellationZones.RemoveAt(i);
                _renderer.DestroyMesh(zone.Mesh);
                UpdateChunkGrid();
            }

            ImGui.PopID();

            ImGui.Separator();
            ImGui.Separator();
        }

        if (ImGui.Button("+"))
        {
            _tessellationZones.Add(new TesselationZone(1, 0, _renderer));
            UpdateChunkGrid();
        }

        ImGui.End();
    }
}
