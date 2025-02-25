﻿using Engine.Graphics;

namespace TerrainGenerator.Vertices;
internal struct BoxVertex : IVertex
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

    public readonly Vector3 GetPosition() => Position;
}
