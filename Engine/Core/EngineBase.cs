using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Engine.Graphics;
using System.Reflection.Metadata;
using Engine.Exceptions;
using Engine.Core.Managers;

namespace Engine.Core;

public class EngineBase : GameWindow
{
    private static bool _wasAlreadyCreated = false;

    private readonly TimeManager _timeManager;

    // TODO maybe move this to a Manager class
    protected new string Title { get; set; } = "My Game";
    protected Vector3 ClearColor { get; set; } = new(0.2f, 0.3f, 0.3f);

    protected Camera _camera = new();

    public EngineBase()
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        if (_wasAlreadyCreated) throw new MultipleSingletonException<EngineBase>();
        else _wasAlreadyCreated = true;

        _timeManager = new TimeManager();
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

        var newSecond = _timeManager.Update(args.Time);

        if (newSecond)
        {
            base.Title = $"{Title} ({_timeManager.FrameRate} fps)";
        }

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
}
