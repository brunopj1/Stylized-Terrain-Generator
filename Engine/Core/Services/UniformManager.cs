using OpenTK.Mathematics;

namespace Engine.Core.Services;

public class UniformManager
{
    // Time
    public double TotalTime { get; internal set; }
    public double DeltaTime { get; internal set; }
    public ulong CurrentFrame { get; internal set; }

    // Matrices
    public Matrix4 ViewMatrix { get; internal set; }
    public Matrix4 ProjectionMatrix { get; internal set; }
    public Matrix4 NormalMatrix { get; internal set; }
}
