using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Services;
using Engine.Graphics;
using TerrainGenerator.Graphics;

namespace TerrainGenerator.Terrain;

internal static class Generator
{
    public static Mesh CreateChunkMesh(Renderer renderer, int divisions)
    {
        var capacity = divisions * divisions * 2 * 3;
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
