using Engine.Exceptions;
using OpenTK.Graphics.OpenGL4;

namespace Engine.Graphics;
public class Shader : IDisposable
{
    private readonly int _handle;
    public int Handle => _handle;

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

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public virtual void BindCustomUniforms() 
    {
    }

    public void Dispose()
    {
        GL.DeleteProgram(_handle);
    }
}
