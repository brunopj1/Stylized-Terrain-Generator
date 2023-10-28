using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using TerrainGenerator.Engine.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace Engine;

public class EngineBase : GameWindow
{
    private readonly FpsCounter _fpsCounter = new();

    protected new string Title { get; set; } = "My Game";
    protected Vector3 ClearColor { get; set; } = new(0.2f, 0.3f, 0.3f);

    public EngineBase()
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (_fpsCounter.Update(args.Time))
        {
            base.Title = $"{Title} ({_fpsCounter.FrameRate} fps)";
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
