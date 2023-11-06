using Engine.Exceptions;

namespace Engine.Graphics;

public class Shader
{
    internal Shader(string vertPath, string? tesscPath, string? tessePath, string? geomPath, string fragPath)
    {
        _vertPath = vertPath;
        _tescPath = tesscPath;
        _tesePath = tessePath;
        _geomPath = geomPath;
        _fragPath = fragPath;
    }

    private readonly string? _vertPath;
    private readonly string? _tescPath;
    private readonly string? _tesePath;
    private readonly string? _geomPath;
    private readonly string? _fragPath;

    private int _handle = -1;
    private readonly Dictionary<string, int> _uniformHandles = new();

    public void Compile()
    {
        if (_handle != -1) Dispose();

        var vertShader = _vertPath != null ? CompileShader(_vertPath, ShaderType.VertexShader) : -1;
        var tescShader = _tescPath != null ? CompileShader(_tescPath, ShaderType.TessControlShader) : -1;
        var teseShader = _tesePath != null ? CompileShader(_tesePath, ShaderType.TessEvaluationShader) : -1;
        var geomShader = _geomPath != null ? CompileShader(_geomPath, ShaderType.GeometryShader) : -1;
        var fragShader = _fragPath != null ? CompileShader(_fragPath, ShaderType.FragmentShader) : -1;

        _handle = GL.CreateProgram();
        if (vertShader != -1) GL.AttachShader(_handle, vertShader);
        if (tescShader != -1) GL.AttachShader(_handle, tescShader);
        if (teseShader != -1) GL.AttachShader(_handle, teseShader);
        if (geomShader != -1) GL.AttachShader(_handle, geomShader);
        if (fragShader != -1) GL.AttachShader(_handle, fragShader);
        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var success);
        if (success == 0)
        {
            var infoLog = GL.GetProgramInfoLog(_handle);
            throw new ShaderCompilationException(infoLog);
        }

        if (vertShader != -1) GL.DetachShader(_handle, vertShader);
        if (tescShader != -1) GL.DetachShader(_handle, tescShader);
        if (teseShader != -1) GL.DetachShader(_handle, teseShader);
        if (geomShader != -1) GL.DetachShader(_handle, geomShader);
        if (fragShader != -1) GL.DetachShader(_handle, fragShader);

        if (vertShader != -1) GL.DeleteShader(vertShader);
        if (tescShader != -1) GL.DeleteShader(tescShader);
        if (teseShader != -1) GL.DeleteShader(teseShader);
        if (geomShader != -1) GL.DeleteShader(geomShader);
        if (fragShader != -1) GL.DeleteShader(fragShader);
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

    public void BindUniform(string name, bool value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, value ? 1 : 0);
    }

    public void BindUniform(string name, int value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, value);
    }

    public void BindUniform(string name, uint value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, value);
    }

    public void BindUniform(string name, float value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, value);
    }

    public void BindUniform(string name, Vector2i value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.Uniform2(uniformHandle, ref value);
    }

    public void BindUniform(string name, Vector3i value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.Uniform3(uniformHandle, ref value);
    }

    public void BindUniform(string name, Vector4i value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.Uniform4(uniformHandle, ref value);
    }

    public void BindUniform(string name, Vector2 value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.Uniform2(uniformHandle, ref value);
    }

    public void BindUniform(string name, Vector3 value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.Uniform3(uniformHandle, ref value);
    }

    public void BindUniform(string name, Vector4 value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.Uniform4(uniformHandle, ref value);
    }

    public void BindUniform(string name, Matrix2 value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.UniformMatrix2(uniformHandle, false, ref value);
    }

    public void BindUniform(string name, Matrix3 value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.UniformMatrix3(uniformHandle, false, ref value);
    }

    public void BindUniform(string name, Matrix4 value)
    {
        var uniformHandle = GetUniformHandle(name);
        if (uniformHandle != -1) GL.UniformMatrix4(uniformHandle, false, ref value);
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
