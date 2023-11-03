using System.Numerics;
using Engine.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace TerrainGenerator.Vertices;
internal struct BoxVertex
{
    public Vector3 Position { set; get; }
    public Vector2 TexCoord { set; get; }

    public static VertexLayout GetLayout()
    {
        return new VertexLayout(new VertexAttribute[]
        {
            new VertexAttribute(VertexAttribPointerType.Float, 3, false), // Position
            new VertexAttribute(VertexAttribPointerType.Float, 2, false)  // Color
        });
    }
}
