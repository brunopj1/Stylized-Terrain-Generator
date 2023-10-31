using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core.Services.Uniforms;

internal class UniformManager : IUniformAccessor
{
    // Time
    public double TotalTime { get; set; }
    public double DeltaTime { get; set; }
    public ulong CurrentFrame { get; set; }

    // Matrices
    public Matrix4 ViewMatrix { get; set; }
    public Matrix4 ProjectionMatrix { get; set; }
    public Matrix4 NormalMatrix { get; set; }

}
