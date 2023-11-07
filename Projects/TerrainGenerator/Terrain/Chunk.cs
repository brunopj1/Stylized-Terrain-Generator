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
    public float TesselationPosX { get; set; }
    public float TesselationNegX { get; set; }
    public float TesselationPosZ { get; set; }
    public float TesselationNegZ { get; set; }

    public void UpdateTesselations(float value)
    {
        Tesselation = value;
        TesselationPosX = value;
        TesselationNegX = value;
        TesselationPosZ = value;
        TesselationNegZ = value;
    }
}
