using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Services;
using Engine.Graphics;
using TerrainGenerator.Services;

namespace TerrainGenerator.Entities;
internal class TesselationZone
{
    public TesselationZone(uint distance, uint divisions, TerrainManager terrainManager)
    {
        Distance = distance;
        Divisions = divisions;
        Mesh = terrainManager.CreateChunkMesh(Divisions);
    }

    public uint Distance { get; set; }
    public uint Divisions { get; set; }
    public Mesh Mesh { get; set; }
}
