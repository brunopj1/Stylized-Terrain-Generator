using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Exceptions;

internal class ShaderException : Exception
{
    public ShaderException(string path, ShaderType type, string info)
        : base($"Failed to compile shader \"{path}\" of type {type}. {info}")
    {
    }

    public ShaderException(string info)
        : base($"Failed to link shader program. {info}")
    {
    }
}
