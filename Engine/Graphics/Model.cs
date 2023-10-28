using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics;

public class Model : IDisposable
{
    private readonly int _vertexBuffer;
    private readonly int _vertexArray;
    private readonly int _vertexCount;

    public Model(float[] vertices)
    {
        if (vertices == null || vertices.Length == 0)
        {
            throw new ArgumentException("Invalid vertices array.");
        }

        _vertexCount = vertices.Length / 6;

        GL.GenVertexArrays(1, out _vertexArray);
        GL.BindVertexArray(_vertexArray);

        GL.GenBuffers(1, out _vertexBuffer);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);

        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

        // Position attribute
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Color attribute
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        //GL.BindVertexArray(0);
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
