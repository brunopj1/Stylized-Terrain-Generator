using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Graphics;
using TerrainGenerator.Terrain.Entities;

namespace TerrainGenerator.Extensions;
internal static class TessellationZoneExtensions
{
    public static TesselationZone GetCorrespondingZone(this List<TesselationZone> zones, Vector2i localOffset)
    {
        var centerDistance = (uint)MathF.Max(MathF.Abs(localOffset.X), MathF.Abs(localOffset.Y));

        foreach (var zone in zones)
        {
            if (centerDistance <= zone.Distance)
            {
                return zone;
            }
            centerDistance -= zone.Distance;
        }

        throw new ArgumentOutOfRangeException("No zone found for the given offset.");
    }
}
