using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Services;
using Engine.Graphics;
using TerrainGenerator.Services;

namespace TerrainGenerator.Terrain.Entities;
internal class TesselationZone
{
    public TesselationZone(uint distance, uint divisions, TerrainManager terrainManager)
    {
        Distance = distance;
        DivisionsLog2 = divisions;
        Mesh = terrainManager.CreateChunkMesh(Divisions);
    }

    public uint Distance { get; set; }
    public uint DivisionsLog2 { get; set; }
    public uint Divisions => (uint)MathF.Pow(2, DivisionsLog2);
    public Mesh Mesh { get; set; }
}
