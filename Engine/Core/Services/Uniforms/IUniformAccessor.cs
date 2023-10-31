using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core.Services.Uniforms;

public interface IUniformAccessor
{
    // Time
    double TotalTime { get; }
    double DeltaTime { get; }
    ulong CurrentFrame { get; }

    // Matrices
    Matrix4 ViewMatrix { get; }
    Matrix4 ProjectionMatrix { get; }
    Matrix4 NormalMatrix { get; }
}
