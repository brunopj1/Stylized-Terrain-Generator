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
        Mesh = CreateMesh(renderer);
    }

    public uint Distance { get; set; }
    public uint DivisionsLog2 { get; set; }
    public uint Divisions => (uint)MathF.Pow(2, DivisionsLog2);
    public Mesh Mesh { get; private set; }

    public void UpdateMesh(Renderer renderer)
    {
        renderer.DestroyMesh(Mesh);
        Mesh = CreateMesh(renderer);
    }

    private Mesh CreateMesh(Renderer renderer)
    {
        var divisions = Divisions;

        var capacity = (int)(divisions * divisions * 2 * 3);
        var vertices = new TerrainVertex[capacity];

        var idx = 0;
        for (var i = 0; i < divisions; i++)
        {
            for (var j = 0; j < divisions; j++)
            {
                // Triangle 0
                vertices[idx++] = new()
                {
                    Position = new((float)i / divisions, (float)j / divisions),
                    TexCoord = new((float)(3 * i + 1) / (3 * divisions), (float)(3 * j + 1) / (3 * divisions)),
                    TriangleIdx = 0
                };
                vertices[idx++] = new()
                {
                    Position = new((float)i / divisions, (float)(j + 1) / divisions),
                    TexCoord = new((float)(3 * i + 1) / (3 * divisions), (float)(3 * j + 1) / (3 * divisions)),
                    TriangleIdx = 0
                };
                vertices[idx++] = new()
                {
                    Position = new((float)(i + 1) / divisions, (float)j / divisions),
                    TexCoord = new((float)(3 * i + 1) / (3 * divisions), (float)(3 * j + 1) / (3 * divisions)),
                    TriangleIdx = 0
                };

                // Triangle 1
                vertices[idx++] = new()
                {
                    Position = new((float)(i + 1) / divisions, (float)(j + 1) / divisions),
                    TexCoord = new((float)(3 * i + 2) / (3 * divisions), (float)(3 * j + 1) / (3 * divisions)),
                    TriangleIdx = 1
                };
                vertices[idx++] = new()
                {
                    Position = new((float)(i + 1) / divisions, (float)j / divisions),
                    TexCoord = new((float)(3 * i + 2) / (3 * divisions), (float)(3 * j + 1) / (3 * divisions)),
                    TriangleIdx = 1
                };
                vertices[idx++] = new()
                {
                    Position = new((float)i / divisions, (float)(j + 1) / divisions),
                    TexCoord = new((float)(3 * i + 2) / (3 * divisions), (float)(3 * j + 1) / (3 * divisions)),
                    TriangleIdx = 1
                };
            }
        }

        return renderer.CreateMesh(vertices, TerrainVertex.GetLayout());
    }
}
