﻿using Engine.Core.Services;
using Engine.Graphics;
using TerrainGenerator;
using TerrainGenerator.Entities;
using TerrainGenerator.Services;

namespace TerrainGenerator.Entities;

internal class Chunk : ICustomUniformManager
{
    public Vector2i Offset { get; set; }
    public uint Divisions { get; set; }

    public Model Model { get; set; } = null;
    public Texture Texture { get; set; } = null;

    public void BindUniforms(AShader shader)
    {
        shader.BindUniform("uChunkOffset", Offset);
        shader.BindUniform("uChunkDivisions", Divisions);
        shader.BindUniform("uChunkTexture", Texture, 0);
    }
}
