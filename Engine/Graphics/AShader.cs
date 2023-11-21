using System.Text.RegularExpressions;
using Engine.Exceptions;

namespace Engine.Graphics;

public abstract partial class AShader
{
    protected int _handle = -1;
    private readonly Dictionary<string, int> _uniformHandles = new();

    [GeneratedRegex("^\\s*(#include\\s+\"([^\"]+)\").*$")]
    private static partial Regex GlslIncludeRegex();

    public void Compile()
    {
        if (_handle != -1) Dispose();

        CompileInternal();

        SetupUniforms();
    }

    protected abstract void CompileInternal();

    protected static int CompileShader(string path, ShaderType type)
    {
        var shaderSource = ReadShaderFile(type, path);
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

    private static string ReadShaderFile(ShaderType type, string path)
    {
        return ReadShaderFileInternal(type, path, null, GlslIncludeRegex(), new(), new());
    }

    private static string ReadShaderFileInternal(ShaderType type, string path, string? parentPath, Regex regex, HashSet<string> alreadyIncluded, List<string> includeStack)
    {
        var combinedPath = parentPath != null ? Path.Combine(Path.GetDirectoryName(parentPath) ?? "", path) : path;
        var fullPath = new Uri(Path.GetFullPath(combinedPath)).LocalPath;

        if (includeStack.Contains(fullPath))
        {
            var message = $"Circular include detected:\n";
            foreach (var p in includeStack) message += $" -> \"{p}\"\n";
            message += $" -> \"{fullPath}\"";

            throw new ShaderCompilationException(includeStack[0], type, message);
        }
        includeStack.Add(fullPath);

        if (alreadyIncluded.Contains(fullPath)) return "";
        alreadyIncluded.Add(fullPath);

        var content = File.ReadAllText(fullPath).Split("\n");

        for (var i = 0; i < content.Length; i++)
        {
            var match = regex.Match(content[i]);
            if (!match.Success) continue;

            var includePath = match.Groups[2].Value;
            var includeContent = ReadShaderFileInternal(type, includePath, fullPath, regex, alreadyIncluded, includeStack);
            content[i] = content[i].Replace(match.Groups[1].Value, $"\n{includeContent}\n");
        }

        includeStack.RemoveAt(includeStack.Count - 1);

        return string.Join("\n", content);
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
        GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var activeUniforms);

        for (var i = 0; i < activeUniforms; i++)
        {
            GL.GetActiveUniform(_handle, i, 256, out _, out _, out _, out var name);
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
