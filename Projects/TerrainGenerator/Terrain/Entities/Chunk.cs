using Engine.Graphics;
using TerrainGenerator;
using TerrainGenerator.Terrain;
using TerrainGenerator.Terrain.Entities;

namespace TerrainGenerator.Terrain.Entities;

internal class Chunk
{
    public Chunk(Model model)
    {
        Model = model;
    }

    public Model Model { get; private set; }
    public Vector2i Offset { get; set; }
    public uint Divisions { get; set; }
}
