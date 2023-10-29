using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Engine.Graphics;

public class Model<T> : IDisposable where T : struct
{
    private readonly int _vertexBuffer;
    private readonly int _vertexArray;
    private readonly int _vertexCount;

    public Model(T[] vertices, VertexLayout layout, BufferUsageHint usageHint, int vertexCount = -1)
    {
        if (vertices == null || vertices.Length == 0)
        {
            throw new ArgumentException("Invalid vertex array.");
        }

        if (vertexCount < 0) vertexCount = vertices.Length;

        var vertexSize = layout.GetVertexSize();
        var expectedSize = vertexSize * vertexCount;
        var dataSize = vertices.Length * Marshal.SizeOf<T>();
        if (dataSize != expectedSize)
        {
            throw new ArgumentException($"Invalid vertex array size: expected {expectedSize} bytes, got {dataSize} bytes.");
        }

        _vertexCount = vertexCount;

        GL.GenVertexArrays(1, out _vertexArray);
        GL.BindVertexArray(_vertexArray);

        GL.GenBuffers(1, out _vertexBuffer);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);

        GL.BufferData(BufferTarget.ArrayBuffer, dataSize, vertices, usageHint);

        var idx = 0;
        var offset = 0;
        foreach (var attr in layout.Attributes)
        {
            GL.VertexAttribPointer(idx, attr.Size, attr.Type, attr.Normalized, vertexSize, offset);
            GL.EnableVertexAttribArray(idx);

            idx++;
            offset += attr.Stride;
        }
    }

    public void Render()
    {
        GL.BindVertexArray(_vertexArray);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
        //GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vertexBuffer);
        GL.DeleteVertexArray(_vertexArray);
    }
}
