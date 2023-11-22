using Engine.Graphics;
using TerrainGenerator;
using TerrainGenerator.Graphics;

namespace TerrainGenerator.Graphics;
internal struct SkyVertex : IVertex
{
    public SkyVertex(Vector2 position)
    {
        Position = position;
    }

    public Vector2 Position { get; set; }

    public static VertexLayout GetLayout()
    {
        return new VertexLayout(new VertexAttribute[]
        {
            new VertexAttribute(VertexAttribPointerType.Float, 2, false) // Position
        });
    }

    public readonly Vector3 GetPosition() => new(Position.X, Position.Y, 0);
}
