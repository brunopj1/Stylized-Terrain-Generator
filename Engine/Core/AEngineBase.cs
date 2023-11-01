using Engine.Core.Controllers;
using Engine.Core.Services;
using Engine.Exceptions;
using Engine.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Core;

public abstract class AEngineBase : GameWindow
{
    public AEngineBase()
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        if (s_wasAlreadyCreated) throw new MultipleEnginesException();
        else s_wasAlreadyCreated = true;
    }

    // Singleton
    private static bool s_wasAlreadyCreated = false;

    // Window
    public new string Title { get; set; } = "My Game";
    public Vector3 ClearColor { get; set; } = new(0.2f, 0.3f, 0.3f);

    // Services
    public EngineClock EngineClock { get; private set; } = new();
    public UniformManager UniformManager { get; private set; } = new();

    // Player controller
    public IPlayerController? PlayerController { get; set; } = null;

    // Rendering objects
    public Camera Camera { get; set; } = new();

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DepthTest);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        Camera.AspectRatio = (float)e.Width / e.Height;
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        var newSecond = EngineClock.Update((float)args.Time);
        if (newSecond)
        {
            base.Title = $"{Title} ({EngineClock.FrameRate} fps)";
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        PlayerController?.Update((float)args.Time);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, 1.0f);

        UpdateUniforms();
    }

    private void UpdateUniforms()
    {
        // Time
        UniformManager.TotalTime = EngineClock.TotalTime;
        UniformManager.DeltaTime = EngineClock.DeltaTime;
        UniformManager.CurrentFrame = EngineClock.CurrentFrame;

        // Matrices
        UniformManager.ViewMatrix = Camera.GetViewMatrix();
        UniformManager.ProjectionMatrix = Camera.GetProjectionMatrix();
        UniformManager.NormalMatrix = Camera.GetNormalMatrix();
    }
}
