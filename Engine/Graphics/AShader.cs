﻿using Engine.Exceptions;

namespace Engine.Graphics;

public abstract class AShader
{
    protected int _handle = -1;
    private readonly Dictionary<string, int> _uniformHandles = new();

    public void Compile()
    {
        if (_handle != -1) Dispose();

        CompileInternal();

        SetupUniforms();
    }

    protected abstract void CompileInternal();

    protected static int CompileShader(string path, ShaderType type)
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

    protected static int CreateProgram(IEnumerable<int> shaders)
    {
        var handle = GL.CreateProgram();

        foreach (var shader in shaders)
        {
            GL.AttachShader(handle, shader);
        }

        GL.LinkProgram(handle);

        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out var success);
        if (success == 0)
        {
            var infoLog = GL.GetProgramInfoLog(handle);
            throw new ShaderCompilationException(infoLog);
        }

        return handle;
    }

    protected void SetupUniforms()
    {
        GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out int activeUniforms);

        for (var i = 0; i < activeUniforms; i++)
        {
            GL.GetActiveUniform(_handle, i, 256, out _, out _, out _, out string name);
            var location = GL.GetUniformLocation(_handle, name);
            _uniformHandles[name] = location;
        }
    }

    protected static void DeleteShaders(int handle, IEnumerable<int> shaders)
    {
        foreach (var shader in shaders)
        {
            GL.DetachShader(handle, shader);
            GL.DeleteShader(shader);
        }
    }

    public void Dispose()
    {
        GL.DeleteProgram(_handle);
        _uniformHandles.Clear();
        _handle = -1;
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public void BindUniform(string name, bool value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, value ? 1 : 0);
    }

    public void BindUniform(string name, int value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, value);
    }

    public void BindUniform(string name, uint value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, value);
    }

    public void BindUniform(string name, float value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, value);
    }

    public void BindUniform(string name, Vector2i value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.Uniform2(uniformHandle, ref value);
    }

    public void BindUniform(string name, Vector3i value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.Uniform3(uniformHandle, ref value);
    }

    public void BindUniform(string name, Vector4i value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.Uniform4(uniformHandle, ref value);
    }

    public void BindUniform(string name, Vector2 value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.Uniform2(uniformHandle, ref value);
    }

    public void BindUniform(string name, Vector3 value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.Uniform3(uniformHandle, ref value);
    }

    public void BindUniform(string name, Vector4 value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.Uniform4(uniformHandle, ref value);
    }

    public void BindUniform(string name, Matrix2 value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.UniformMatrix2(uniformHandle, false, ref value);
    }

    public void BindUniform(string name, Matrix3 value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.UniformMatrix3(uniformHandle, false, ref value);
    }

    public void BindUniform(string name, Matrix4 value)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1) GL.UniformMatrix4(uniformHandle, false, ref value);
    }

    public void BindUniform(string name, Texture texture, int textureUnit)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1)
        {
            texture.BindTexture(textureUnit);
            GL.Uniform1(uniformHandle, textureUnit);
        }
    }

    public void BindUniform(string name, Texture texture, int textureUnit, TextureAccess access)
    {
        var uniformHandle = _uniformHandles.GetValueOrDefault(name, -1);
        if (uniformHandle != -1)
        {
            texture.BindImageTexture(textureUnit, access);
            GL.Uniform1(uniformHandle, textureUnit);
        }
    }
}
