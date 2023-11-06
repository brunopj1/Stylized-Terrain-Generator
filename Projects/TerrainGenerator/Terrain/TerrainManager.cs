﻿using Engine.Core.Services;
using Engine.Graphics;
using ImGuiNET;
using TerrainGenerator.Terrain;

namespace TerrainGenerator.Services;

internal class TerrainManager : ICustomUniformManager
{
    public TerrainManager(Renderer renderer, Mesh mesh, Shader shader)
    {
        _renderer = renderer;
        _mesh = mesh;
        _shader = shader;

        _tesselationLevels = new()
        {
            new TesselationLevel{ Distance = 4, Tesselation = 16 },
            new TesselationLevel{ Distance = 8, Tesselation = 8 },
            new TesselationLevel{ Distance = 16, Tesselation = 4 },
        };

        // Fake initialization
        _chunkGrid = new Chunk[0, 0];
        _currentChunk = null;
        _gridOffset = new();

        UpdateChunkGrid();
        UpdateCameraViewDistance();
    }

    private readonly Renderer _renderer;
    private readonly Mesh _mesh;
    private readonly Shader _shader;

    private Chunk[,] _chunkGrid;
    private Chunk? _currentChunk;
    private Vector2i _gridOffset;

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
            UpdateChunkModels();
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
            UpdateChunkModels();
        }
    }

    private readonly List<TesselationLevel> _tesselationLevels;

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

            UpdateChunkOffsets();
        }
    }

    private void UpdateChunkGrid()
    {
        foreach (var chunk in _chunkGrid)
        {
            _renderer.DestroyModel(chunk.Model);
        }

        var size = _chunkRadius * 2 + 1;
        _chunkGrid = new Chunk[size, size];

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                var mesh = _renderer.CreateModel(_mesh, _shader, customUniformManager: this);
                _chunkGrid[i, j] = new(mesh);
            }
        }

        UpdateChunkModels();
        UpdateChunkOffsets();
        UpdateChunkTesselations();
    }

    private void UpdateChunkModels()
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

    private void UpdateChunkTesselations()
    {
        for (var i = 0; i < _chunkGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _chunkGrid.GetLength(1); j++)
            {
                var chunk = _chunkGrid[i, j];

                var localOffset = new Vector2i(i - (int)_chunkRadius, j - (int)_chunkRadius);
                var centerDistance = MathF.Max(MathF.Abs(localOffset.X), MathF.Abs(localOffset.Y));
                //var centerDistance = localOffset.ManhattanLength;

                // TODO outer tesselation

                chunk.Tesselation = 1;
                foreach (var level in _tesselationLevels)
                {
                    if (centerDistance <= level.Distance)
                    {
                        chunk.Tesselation = level.Tesselation;
                        break;
                    }
                }
            }
        }
    }

    private void UpdateCameraViewDistance()
    {
        _renderer.Camera.Far = MathF.Max((_chunkRadius + 1) * 1.2f * _chunkLength, 1000);
        _renderer.Camera.Near = _renderer.Camera.Far * 0.001f;
    }

    public void BindUniforms(Shader shader)
    {
        shader.BindUniform("uChunkLength", _chunkLength);
        shader.BindUniform("uChunkHeight", _chunkHeight);

        shader.BindUniform("uChunkOffset", _currentChunk!.Offset);
        shader.BindUniform("uChunkTesselation", _currentChunk!.Tesselation);

        shader.BindUniform("uTerrainFrequency", _terrainFrequency);
    }

    public void Render(EngineUniformManager uniformManager)
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
        RenderTesselationSettingsWindow();
    }

    private void RenderTerrainSettingsWindow()
    {
        ImGui.Begin("Grid Settings");

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

    private void RenderTesselationSettingsWindow()
    {
        var modified = false;

        ImGui.Begin("Tesselation Settings");

        for (var i = 0; i < _tesselationLevels.Count; i++)
        {
            var level = _tesselationLevels[i];

            ImGui.PushID(i);

            var tempI = level.Distance;
            if (ImGui.DragInt("Distance", ref tempI, 1, 1, 50))
            {
                level.Distance = tempI;
                modified = true;
            }

            tempI = level.Tesselation;
            if (ImGui.DragInt("Tesselation", ref tempI, 0.1f, 1, 64))
            {
                level.Tesselation = tempI;
                modified = true;
            }

            if (ImGui.Button(@"/\") && i != 0)
            {
                var temp = _tesselationLevels[i - 1];
                _tesselationLevels[i - 1] = level;
                _tesselationLevels[i] = temp;
                modified = true;
                break;
            }

            ImGui.SameLine();

            if (ImGui.Button(@"\/") && i != _tesselationLevels.Count - 1)
            {
                var temp = _tesselationLevels[i + 1];
                _tesselationLevels[i + 1] = level;
                _tesselationLevels[i] = temp;
                modified = true;
                break;
            }

            ImGui.SameLine();

            if (ImGui.Button("X"))
            {
                _tesselationLevels.RemoveAt(i);
                modified = true;
                break;
            }

            ImGui.PopID();

            ImGui.Separator();
            ImGui.Separator();
        }

        if (ImGui.Button("+"))
        {
            _tesselationLevels.Add(new TesselationLevel());
            modified = true;
        }

        ImGui.End();

        if (modified) UpdateChunkTesselations();
    }
}
