using Engine.Core.Services;
using Engine.Graphics;
using TerrainGenerator;
using TerrainGenerator.Terrain;
using TerrainGenerator.Terrain.Entities;

namespace TerrainGenerator.Terrain.Entities;

internal class Chunk
{
    public Vector2i Offset { get; set; }
    public uint Divisions { get; set; }

    public Model Model { get; set; } = null;
    public Texture HightmapTexture { get; set; } = null;
    public Texture ColormapTexture { get; set; } = null;
}
