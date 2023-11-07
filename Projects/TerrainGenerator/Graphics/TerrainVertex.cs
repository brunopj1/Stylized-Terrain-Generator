using Engine.Graphics;
using TerrainGenerator;
using TerrainGenerator.Graphics;
using TerrainGenerator.Terrain;

namespace TerrainGenerator.Graphics;
internal struct TerrainVertex : IVertex
{
    public Vector2 Position { get; set; }

    public static VertexLayout GetLayout()
    {
        return new VertexLayout(new VertexAttribute[]
        {
            new VertexAttribute(VertexAttribPointerType.Float, 2, false) // Position
        });
    }

    public readonly Vector3 GetPosition() => new(Position.X, 0f, Position.Y);
}
