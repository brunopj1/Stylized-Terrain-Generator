using OpenTK.Graphics.OpenGL4;

namespace Engine.Exceptions;

internal class ShaderException : Exception
{
    public ShaderException(string path, ShaderType type, string info)
        : base($"Failed to compile {type} located in \"{path}\".\n{info}")
    {
    }

    public ShaderException(string info)
        : base($"Failed to link shader program.\n{info}")
    {
    }
}