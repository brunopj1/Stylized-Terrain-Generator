using Engine.Graphics;

namespace Engine.Core.Services;

public class EngineUniformManager
{
    // Time
    public double TotalTime { get; internal set; }
    public double DeltaTime { get; internal set; }
    public ulong CurrentFrame { get; internal set; }

    // Matrices
    private Matrix4 _viewMatrix;
    public Matrix4 ViewMatrix
    {
        get => _viewMatrix;
        internal set => _viewMatrix = value;
    }

    private Matrix4 _projectionMatrix;
    public Matrix4 ProjectionMatrix
    {
        get => _projectionMatrix;
        internal set => _projectionMatrix = value;
    }

    private Matrix4 _normalMatrix;
    public Matrix4 NormalMatrix
    {
        get => _normalMatrix;
        internal set => _normalMatrix = value;
    }

    internal void BindUniforms(RenderShader shader, ref Matrix4 modelMatrix)
    {
        // Time
        shader.BindUniform("uTotalTime", (float)TotalTime);

        shader.BindUniform("uDeltaTime", (float)DeltaTime);

        shader.BindUniform("uCurrentFrame", CurrentFrame);

        // Matrices
        shader.BindUniform("uModelMatrix", modelMatrix);

        shader.BindUniform("uViewMatrix", _viewMatrix);

        shader.BindUniform("uProjectionMatrix", _projectionMatrix);

        shader.BindUniform("uNormalMatrix", _normalMatrix);

        var pvmMatrix = modelMatrix * _viewMatrix * _projectionMatrix;
        shader.BindUniform("uPVMMatrix", pvmMatrix);
    }
}
