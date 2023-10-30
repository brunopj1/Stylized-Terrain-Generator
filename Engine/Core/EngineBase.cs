using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Engine.Graphics;
using System.Reflection.Metadata;
using Engine.Exceptions;

namespace Engine.Core;

public class EngineBase : GameWindow
{
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

    // Rendering objects
    private readonly EngineUniformManager _uniformManager = new();
    protected IEngineUniformAccessor UniformAccessor => _uniformManager;

    protected Camera _camera = new();

    public EngineBase()
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        if (_wasAlreadyCreated) throw new MultipleEnginesException();
        else _wasAlreadyCreated = true;
    }

    // GameWindow methods

    protected override void OnLoad()
    {
        base.OnLoad();
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

        UpdateUniforms();

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, 1.0f);
    }

    // Internal methods
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
