using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrainGenerator.Services;

namespace TerrainGenerator.Terrain.Entities;

internal class TesselationMap : List<TesselationZone>
{
    public TesselationMap(params TesselationZone[] zones)
    {
        this.AddRange(zones);
    }

    public uint TotalRadius => (uint)this.Sum(zone => zone.Distance);
    public uint MaxDivisions => (uint)MathF.Pow(2, this.Max(zone => zone.DivisionsLog2));

    public TesselationZone GetCorrespondingZone(Vector2i localOffset)
    {
        var centerDistance = (uint)MathF.Max(MathF.Abs(localOffset.X), MathF.Abs(localOffset.Y));

        foreach (var zone in this.AsEnumerable())
        {
            if (centerDistance <= zone.Distance)
            {
                return zone;
            }
            centerDistance -= zone.Distance;
        }

        throw new ArgumentOutOfRangeException();
    }
}
