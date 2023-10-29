using OpenTK.Graphics.OpenGL4;

namespace Engine.Graphics;

public struct VertexAttribute
{
    public VertexAttribPointerType Type { get; private set; }
    public int Size { get; private set; }
    public bool Normalized { get; private set; }
    public int Stride { get; private set; }

    public VertexAttribute(VertexAttribPointerType type, int size, bool normalized)
    {
        Type = type;
        Size = size;
        Normalized = normalized;
        Stride = Size * type switch
        {
            VertexAttribPointerType.Byte => sizeof(byte),
            VertexAttribPointerType.UnsignedByte => sizeof(byte),
            VertexAttribPointerType.Short => sizeof(short),
            VertexAttribPointerType.UnsignedShort => sizeof(ushort),
            VertexAttribPointerType.Int => sizeof(int),
            VertexAttribPointerType.UnsignedInt => sizeof(uint),
            VertexAttribPointerType.Float => sizeof(float),
            VertexAttribPointerType.Double => sizeof(double),
            VertexAttribPointerType.HalfFloat => sizeof(ushort),
            VertexAttribPointerType.Fixed => sizeof(int),
            VertexAttribPointerType.UnsignedInt2101010Rev => sizeof(int),
            VertexAttribPointerType.UnsignedInt10F11F11FRev => sizeof(uint),
            VertexAttribPointerType.Int2101010Rev => sizeof(int),
            _ => throw new NotSupportedException("Unsupported VertexAttribPointerType")
        };
    }
}
