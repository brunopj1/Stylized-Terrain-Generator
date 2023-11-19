using Engine.Core.Services;
using Engine.Graphics;
using TerrainGenerator;
using TerrainGenerator.Services;
using TerrainGenerator.Terrain;
using TerrainGenerator.Terrain.Entities;

namespace TerrainGenerator.Terrain.Entities;

internal class Chunk : ICustomUniformManager
{
    public Chunk(TerrainManager terrainManager)
    {
        _terrainManager = terrainManager;
    }

    private readonly TerrainManager _terrainManager;

    public Vector2i Offset { get; set; }
    public uint Divisions { get; set; }

    public Model Model { get; set; } = null;
    public Texture Texture { get; set; } = null;

    public void BindUniforms(AShader shader)
    {
        _terrainManager.BindUniforms(shader);

        shader.BindUniform("uChunkOffset", Offset);
        shader.BindUniform("uChunkDivisions", Divisions);
        shader.BindUniform("uChunkTexture", Texture, 0);
    }
}
