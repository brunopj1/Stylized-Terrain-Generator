using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Engine.Graphics;

public abstract class Mesh
{
    internal abstract void Build();
    internal abstract void Dispose();
    public abstract void Render();
}

public class Mesh<T> : Mesh where T : struct
{
    internal Mesh(T[] vertices, int vertexCount, VertexLayout layout, PrimitiveType primitiveType, BufferUsageHint usageHint)
    {
        _vertices = vertices;
        _vertexCount = vertexCount > 0 ? vertexCount : vertices.Length;
        _layout = layout;

        _primitiveType = primitiveType;
        _usageHint = usageHint;

        if (vertices == null || vertices.Length == 0)
        {
            throw new ArgumentException("Invalid vertex array.");
        }

        var vertexSize = layout.GetVertexSize();
        var expectedSize = vertexSize * _vertexCount;
        var dataSize = vertices.Length * Marshal.SizeOf<T>();
        if (dataSize != expectedSize)
        {
            throw new ArgumentException($"Invalid vertex array size: expected {expectedSize} bytes, got {dataSize} bytes.");
        }
    }

    private readonly T[] _vertices;
    private readonly int _vertexCount;
    private readonly VertexLayout _layout;

    private readonly PrimitiveType _primitiveType = PrimitiveType.Triangles;
    private readonly BufferUsageHint _usageHint = BufferUsageHint.StaticDraw;

    private int _vertexBuffer = -1;
    private int _vertexArray = -1;

    internal override void Build()
    {
        if (_vertexBuffer != -1 || _vertexArray != -1) return;

        var vertexSize = _layout.GetVertexSize();
        var dataSize = vertexSize * _vertexCount;

        GL.GenVertexArrays(1, out _vertexArray);
        GL.BindVertexArray(_vertexArray);

        GL.GenBuffers(1, out _vertexBuffer);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);

        GL.BufferData(BufferTarget.ArrayBuffer, dataSize, _vertices, _usageHint);

        var idx = 0;
        var offset = 0;
        foreach (var attr in _layout.Attributes)
        {
            GL.VertexAttribPointer(idx, attr.Size, attr.Type, attr.Normalized, vertexSize, offset);
            GL.EnableVertexAttribArray(idx);

            idx++;
            offset += attr.Stride;
        }
    }

    internal override void Dispose()
    {
        GL.DeleteBuffer(_vertexBuffer);
        GL.DeleteVertexArray(_vertexArray);
        _vertexBuffer = -1;
        _vertexArray = -1;
    }

    public override void Render()
    {
        GL.BindVertexArray(_vertexArray);
        GL.DrawArrays(_primitiveType, 0, _vertexCount);
    }
}
