using Engine.Graphics;
using TerrainGenerator;
using TerrainGenerator.Graphics;

namespace TerrainGenerator.Graphics;
internal struct TerrainVertex : IVertex
{
    public TerrainVertex(Vector2 position, float triangleIdx)
    {
        Position = position;
        TriangleIdx = triangleIdx;
    }

    public Vector2 Position { get; set; }
    public float TriangleIdx { get; set; }

    public static VertexLayout GetLayout()
    {
        return new VertexLayout(new VertexAttribute[]
        {
            new VertexAttribute(VertexAttribPointerType.Float, 2, false), // Position
            new VertexAttribute(VertexAttribPointerType.Float, 1, false) // TriangleIdx
        });
    }

    public readonly Vector3 GetPosition() => new(Position.X, 0f, Position.Y);
}
