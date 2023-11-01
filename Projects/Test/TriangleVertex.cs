using Engine.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Test;

internal struct TriangleVertex
{
    public Vector3 Position { set; get; }
    public Vector3 Color { set; get; }

    public static VertexLayout GetLayout()
    {
        return new VertexLayout(new VertexAttribute[]
        {
            new VertexAttribute(VertexAttribPointerType.Float, 3, false), // Position
            new VertexAttribute(VertexAttribPointerType.Float, 3, false)  // Color
        });
    }
}