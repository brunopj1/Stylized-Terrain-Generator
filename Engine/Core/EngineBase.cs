using Engine.Core.Services.Uniforms;
using Engine.Exceptions;
using Engine.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Core;

public class EngineBase : GameWindow
{
    public EngineBase()
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        if (_wasAlreadyCreated) throw new MultipleEnginesException();
        else _wasAlreadyCreated = true;
    }

    // Singleton
    private static bool _wasAlreadyCreated = false;

    // Clock
    protected double TotalTime { get; private set; } = 0;
    protected double DeltaTime { get; private set; } = 0;
    protected ulong CurrentFrame { get; private set; } = 0;
    protected int FrameRate { get; private set; } = 0;

    private int _framesThisSecond = 0;
    private double _elapsedTimeThisSecond = 1;

    // Window
    protected new string Title { get; set; } = "My Game";
    protected Vector3 ClearColor { get; set; } = new(0.2f, 0.3f, 0.3f);

    // TODO move this to an input manager
    private Vector2 _previousMousePosition = Vector2i.Zero;
    protected Vector2 MouseOffset { get; private set; } = Vector2i.Zero;

    // Services
    private readonly UniformManager _uniformManager = new();
    protected IUniformAccessor UniformAccessor => _uniformManager;

    // Rendering objects
    protected Camera _camera = new();

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DepthTest);

        _previousMousePosition = MousePosition;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        _camera.AspectRatio = (float)e.Width / e.Height;
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        var newSecond = UpdateClock(args.Time);
        if (newSecond)
        {
            base.Title = $"{Title} ({FrameRate} fps)";
        }

        MouseOffset = MousePosition - _previousMousePosition;
        _previousMousePosition = MousePosition;

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, 1.0f);

        UpdateUniforms();
    }

    private bool UpdateClock(double elapsedTime)
    {
        TotalTime += elapsedTime;
        DeltaTime = elapsedTime;
        CurrentFrame++;

        _elapsedTimeThisSecond += elapsedTime;
        _framesThisSecond++;

        if (_elapsedTimeThisSecond >= 1)
        {
            FrameRate = _framesThisSecond;
            _framesThisSecond = 0;
            _elapsedTimeThisSecond = 0;
            return true;
        }

        return false;
    }

    private void UpdateUniforms()
    {
        // Time
        _uniformManager.TotalTime = TotalTime;
        _uniformManager.DeltaTime = DeltaTime;
        _uniformManager.CurrentFrame = CurrentFrame;

        // Matrices
        _uniformManager.ViewMatrix = _camera.GetViewMatrix();
        _uniformManager.ProjectionMatrix = _camera.GetProjectionMatrix();
        _uniformManager.NormalMatrix = _camera.GetNormalMatrix();
    }
}