using Engine.Graphics;
using OpenTK.Windowing.Desktop;

namespace Engine.Core.Services.Internal;

internal class EngineUniformManager
{
    public EngineUniformManager(GameWindow window, EngineClock clock, Renderer renderer)
    {
        _window = window;
        _clock = clock;
        _renderer = renderer;
    }

    private readonly GameWindow _window;
    private readonly EngineClock _clock;
    private readonly Renderer _renderer;

    internal void BindUniforms(RenderShader shader, ref Matrix4 modelMatrix)
    {
        // Time

        shader.BindUniform("uTotalTime", _clock.TotalTime);

        shader.BindUniform("uDeltaTime", _clock.DeltaTime);

        shader.BindUniform("uCurrentFrame", _clock.CurrentFrame);

        // Matrices

        shader.BindUniform("uModelMatrix", modelMatrix);

        var camera = _renderer.Camera;

        if (camera != null)
        {
            shader.BindUniform("uViewMatrix", camera.ViewMatrix);

            shader.BindUniform("uNormalMatrix", camera.NormalMatrix);

            shader.BindUniform("uProjectionMatrix", camera.ProjectionMatrix);

            shader.BindUniform("uPVMMatrix", modelMatrix * camera.ViewMatrix * camera.ProjectionMatrix);
        }

        // Window

        var resolution = _window.Size;

        shader.BindUniform("uWindowResolution", resolution);

        shader.BindUniform("uWindowAspectRatio", resolution.X / (float)resolution.Y);

        // Camera

        if (camera != null)
        {
            shader.BindUniform("uCameraPosition", camera.Position);

            shader.BindUniform("uCameraYaw", camera.Yaw);

            shader.BindUniform("uCameraPitch", camera.Pitch);

            shader.BindUniform("uCameraFront", camera.Front);

            shader.BindUniform("uCameraRight", camera.Right);

            shader.BindUniform("uCameraUp", camera.Up);

            shader.BindUniform("uCameraNear", camera.Near);

            shader.BindUniform("uCameraFar", camera.Far);

            shader.BindUniform("uCameraAspectRatio", camera.AspectRatio);

            shader.BindUniform("uCameraFov", camera.Fov);
        }
    }
}
