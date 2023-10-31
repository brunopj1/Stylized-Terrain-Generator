using Engine.Core.Services.Uniforms;
using Engine.Exceptions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine.Graphics;

public class Shader : IDisposable
{
    public Shader(string vertexPath, string fragmentPath)
    {
        var vertexShader = CompileShader(vertexPath, ShaderType.VertexShader);
        var fragmentShader = CompileShader(fragmentPath, ShaderType.FragmentShader);

        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(_handle);
            throw new ShaderException(infoLog);
        }

        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    private readonly int _handle;
    private readonly Dictionary<string, int> _uniformHandles = new();

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public void Dispose()
    {
        GL.DeleteProgram(_handle);
    }

    public void BindUniforms(IUniformAccessor uniformAccessor, Matrix4 modelMatrix)
    {
        // Time
        var uniformHandle = GetUniformHandle("uTotalTime");
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, (float)uniformAccessor.TotalTime);

        uniformHandle = GetUniformHandle("uDeltaTime");
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, (float)uniformAccessor.DeltaTime);

        uniformHandle = GetUniformHandle("uCurrentFrame");
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, uniformAccessor.CurrentFrame);

        // Matrices
        uniformHandle = GetUniformHandle("uModelMatrix");
        if (uniformHandle != -1) GL.UniformMatrix4(uniformHandle, false, ref modelMatrix);

        uniformHandle = GetUniformHandle("uViewMatrix");
        var viewMatrix = uniformAccessor.ViewMatrix;
        if (uniformHandle != -1) GL.UniformMatrix4(uniformHandle, false, ref viewMatrix);

        uniformHandle = GetUniformHandle("uProjectionMatrix");
        var projectionMatrix = uniformAccessor.ProjectionMatrix;
        if (uniformHandle != -1) GL.UniformMatrix4(uniformHandle, false, ref projectionMatrix);

        uniformHandle = GetUniformHandle("uNormalMatrix");
        var normalMatrix = uniformAccessor.NormalMatrix;
        if (uniformHandle != -1) GL.UniformMatrix4(uniformHandle, false, ref normalMatrix);

        uniformHandle = GetUniformHandle("uPVMMatrix");
        var pvmMatrix = modelMatrix * viewMatrix * projectionMatrix;
        if (uniformHandle != -1) GL.UniformMatrix4(uniformHandle, false, ref pvmMatrix);

        // Custom uniforms
        BindCustomUniforms();
    }

    protected virtual void BindCustomUniforms()
    {
    }

    protected int GetUniformHandle(string name)
    {
        if (_uniformHandles.TryGetValue(name, out var handle)) return handle;

        handle = GL.GetUniformLocation(_handle, name);
        _uniformHandles[name] = handle;
        return handle;
    }

    private int CompileShader(string path, ShaderType type)
    {
        var shaderSource = File.ReadAllText(path);
        var shader = GL.CreateShader(type);
        GL.ShaderSource(shader, shaderSource);
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);

        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new ShaderException(path, type, infoLog);
        }

        return shader;
    }
}