using Engine.Graphics;

namespace TerrainGenerator.Terrain;

internal class Chunk
{
    public Chunk(Model model)
    {
        Model = model;
    }

    public Model Model { get; private set; }
    public Vector2i Offset { get; set; }
    public float Tesselation { get; set; }
}
