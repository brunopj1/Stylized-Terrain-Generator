using System.Runtime.InteropServices;

namespace Engine.Graphics;

public struct MeshParameters
{
    public MeshParameters()
    {
    }

    public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Triangles;
    public BufferUsageHint UsageHint { get; set; } = BufferUsageHint.StaticDraw;
}

public abstract class Mesh
{
    internal abstract void Build();
    internal abstract void Dispose();
    public abstract ulong Render();

    internal abstract BoundingVolume ComputeBoundingVolume();
}

public class Mesh<T> : Mesh where T : struct, IVertex
{
    internal Mesh(T[] vertices, uint[]? indices, VertexLayout layout, MeshParameters? parameters)
    {
        _vertices = vertices;
        _indices = indices;
        _layout = layout;
        _parameters = parameters ?? new();

        if (vertices == null || vertices.Length == 0)
        {
            throw new ArgumentException("Invalid vertex array");
        }

        var vertexSize = layout.GetVertexSize();
        var expectedSize = vertexSize * _vertices.Length;
        var dataSize = vertices.Length * Marshal.SizeOf<T>();
        if (dataSize != expectedSize)
        {
            throw new ArgumentException($"Invalid vertex array size: expected {expectedSize} bytes, got {dataSize} bytes.");
        }
    }

    private readonly T[] _vertices;
    private readonly uint[]? _indices;
    private readonly VertexLayout _layout;
    private readonly MeshParameters _parameters;

    private int _vertexBuffer = -1;
    private int _vertexArray = -1;
    private int _elementBuffer = -1;

    internal override void Build()
    {
        if (_vertexBuffer != -1 || _vertexArray != -1) return;

        var vertexSize = _layout.GetVertexSize();
        var dataSize = vertexSize * _vertices.Length;

        GL.GenVertexArrays(1, out _vertexArray);
        GL.BindVertexArray(_vertexArray);

        GL.GenBuffers(1, out _vertexBuffer);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);

        GL.BufferData(BufferTarget.ArrayBuffer, dataSize, _vertices, _parameters.UsageHint);

        var idx = 0;
        var offset = 0;
        foreach (var attr in _layout.Attributes)
        {
            GL.VertexAttribPointer(idx, attr.Size, attr.Type, attr.Normalized, vertexSize, offset);
            GL.EnableVertexAttribArray(idx);

            idx++;
            offset += attr.Stride;
        }

        if (_indices != null && _indices.Length > 0)
        {
            GL.GenBuffers(1, out _elementBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
        }
    }

    internal override void Dispose()
    {
        GL.DeleteBuffer(_vertexBuffer);
        GL.DeleteVertexArray(_vertexArray);
        _vertexBuffer = -1;
        _vertexArray = -1;
    }

    public override ulong Render()
    {
        if (_elementBuffer != -1)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBuffer);
            GL.BindVertexArray(_vertexArray);
            GL.DrawElements(_parameters.PrimitiveType, _indices!.Length, DrawElementsType.UnsignedInt, 0);
            return (ulong)_indices.Length;
        }
        else
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.BindVertexArray(_vertexArray);
            GL.DrawArrays(_parameters.PrimitiveType, 0, _vertices.Length);
            return (ulong)_vertices.Length;
        }
    }

    internal override BoundingVolume ComputeBoundingVolume()
    {
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);

        foreach (var vertex in _vertices)
        {
            var position = vertex.GetPosition();
            min = Vector3.ComponentMin(min, position);
            max = Vector3.ComponentMax(max, position);
        }

        return new AxisAlignedBoundingBox(min, max);
    }
}
