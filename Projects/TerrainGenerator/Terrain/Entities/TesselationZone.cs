using Engine.Core.Services;
using Engine.Graphics;
using TerrainGenerator;
using TerrainGenerator.Graphics;
using TerrainGenerator.Terrain;
using TerrainGenerator.Terrain.Entities;

namespace TerrainGenerator.Terrain.Entities;

internal class TesselationZone
{
    public TesselationZone(uint distance, uint divisions, Renderer renderer)
    {
        Distance = distance;
        DivisionsLog2 = divisions;
        Mesh = Generator.CreateChunkMesh(renderer, (int)Divisions);
    }

    public uint Distance { get; set; }
    public uint DivisionsLog2 { get; set; }
    public uint Divisions => (uint)MathF.Pow(2, DivisionsLog2);
    public Mesh Mesh { get; private set; }

    public void UpdateMesh(Renderer renderer)
    {
        renderer.DestroyMesh(Mesh);
        Mesh = Generator.CreateChunkMesh(renderer, (int)Divisions);
    }
}
