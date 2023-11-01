using Engine.Core.Services;
using Engine.Exceptions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine.Graphics;

public class Shader
{
    internal Shader(string vertexShaderPath, string fragmentShaderPath)
    {
        _vertexShaderPath = vertexShaderPath;
        _fragmentShaderPath = fragmentShaderPath;
    }

    private readonly string _vertexShaderPath;
    private readonly string _fragmentShaderPath;

    private int _handle = -1;

    private readonly Dictionary<string, int> _uniformHandles = new();

    public void Compile()
    {
        if (_handle != -1) Dispose();

        var vertexShader = CompileShader(_vertexShaderPath, ShaderType.VertexShader);
        var fragmentShader = CompileShader(_fragmentShaderPath, ShaderType.FragmentShader);

        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var success);
        if (success == 0)
        {
            var infoLog = GL.GetProgramInfoLog(_handle);
            throw new ShaderCompilationException(infoLog);
        }

        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    internal void Dispose()
    {
        GL.DeleteProgram(_handle);
        _uniformHandles.Clear();
        _handle = -1;
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    internal void BindUniforms(UniformManager uniformManager, Matrix4 modelMatrix)
    {
        // Time
        var uniformHandle = GetUniformHandle("uTotalTime");
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, (float)uniformManager.TotalTime);

        uniformHandle = GetUniformHandle("uDeltaTime");
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, (float)uniformManager.DeltaTime);

        uniformHandle = GetUniformHandle("uCurrentFrame");
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, uniformManager.CurrentFrame);

        // Matrices
        uniformHandle = GetUniformHandle("uModelMatrix");
        if (uniformHandle != -1) GL.UniformMatrix4(uniformHandle, false, ref modelMatrix);

        uniformHandle = GetUniformHandle("uViewMatrix");
        var viewMatrix = uniformManager.ViewMatrix;
        if (uniformHandle != -1) GL.UniformMatrix4(uniformHandle, false, ref viewMatrix);

        uniformHandle = GetUniformHandle("uProjectionMatrix");
        var projectionMatrix = uniformManager.ProjectionMatrix;
        if (uniformHandle != -1) GL.UniformMatrix4(uniformHandle, false, ref projectionMatrix);

        uniformHandle = GetUniformHandle("uNormalMatrix");
        var normalMatrix = uniformManager.NormalMatrix;
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

    private static int CompileShader(string path, ShaderType type)
    {
        var shaderSource = File.ReadAllText(path);
        var shader = GL.CreateShader(type);
        GL.ShaderSource(shader, shaderSource);
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out var success);

        if (success == 0)
        {
            var infoLog = GL.GetShaderInfoLog(shader);
            throw new ShaderCompilationException(path, type, infoLog);
        }

        return shader;
    }
}
