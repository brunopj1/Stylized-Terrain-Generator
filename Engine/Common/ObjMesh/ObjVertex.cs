using Engine.Graphics;

namespace Engine.Common.ObjMesh;

public struct ObjVertex : IVertex
{
    public Vector3 Position { get; set; }
    public Vector3 Normal { get; set; }
    public Vector2 TexCoord { get; set; }

    public static VertexLayout GetLayout()
    {
        return new VertexLayout(new VertexAttribute[]
        {
            new VertexAttribute(VertexAttribPointerType.Float, 3, false), // Position
            new VertexAttribute(VertexAttribPointerType.Float, 3, false), // Normal
            new VertexAttribute(VertexAttribPointerType.Float, 2, false)  // TexCoord
        });
    }

    public readonly Vector3 GetPosition() => Position;
}
