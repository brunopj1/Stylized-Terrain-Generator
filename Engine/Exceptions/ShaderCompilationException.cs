namespace Engine.Exceptions;

internal class ShaderCompilationException : Exception
{
    public ShaderCompilationException(string path, ShaderType type, string info)
        : base($"Failed to compile {type} located in \"{path}\".\n{info}")
    {
    }

    public ShaderCompilationException(string info)
        : base($"Failed to link shader program.\n{info}")
    {
    }
}
