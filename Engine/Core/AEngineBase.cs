using Engine.Core.Controllers;
using Engine.Core.Internal;
using Engine.Core.Services;
using Engine.Exceptions;
using ImGuiNET;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

namespace Engine.Core;

public abstract class AEngineBase : GameWindow
{
    public AEngineBase()
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        if (s_wasAlreadyCreated) throw new MultipleEnginesException();
        else s_wasAlreadyCreated = true;

        Renderer = new(UniformManager);
        _imGuiLayer = new(this);
    }

    // Singleton
    private static bool s_wasAlreadyCreated = false;

    // Internal
    private ImGuiController? _imGuiController = null;

    // Settings
    public new string Title { get; set; } = "My Game";
    public Vector3 ClearColor { get; set; } = new(0.2f, 0.3f, 0.3f);

    // Public services
    public EngineClock EngineClock { get; private set; } = new();
    public UniformManager UniformManager { get; private set; } = new();
    public Renderer Renderer { get; private set; }

    // Private services
    private readonly ImGuiLayer _imGuiLayer;

    // Player controller
    public IPlayerController? PlayerController { get; set; } = null;

    protected override void OnLoad()
    {
        base.OnLoad();

        _imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);

        StbImage.stbi_set_flip_vertically_on_load(1);

        Renderer.Load();

        GL.Enable(EnableCap.DepthTest);

        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        Renderer.Unload();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        EngineClock.Update((float)args.Time);

        if (EngineClock.NewSecond)
        {
            base.Title = $"{Title} ({(int)ImGui.GetIO().Framerate} fps)";
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
            return;
        }

        PlayerController?.Update((float)args.Time);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        _imGuiController?.Update(this, (float)args.Time);

        UpdateUniforms();

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, 1.0f);

        _imGuiLayer.RenderMenuBar();
    }

    protected void CompleteOnRenderFrame()
    {
        _imGuiController?.Render();

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs args)
    {
        base.OnResize(args);

        GL.Viewport(0, 0, args.Width, args.Height);

        _imGuiController?.WindowResized(ClientSize.X, ClientSize.Y);

        Renderer.Camera.AspectRatio = (float)args.Width / args.Height;
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);

        _imGuiController?.PressChar((char)e.Unicode);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _imGuiController?.MouseScroll(e.Offset);
    }

    private void UpdateUniforms()
    {
        // Time
        UniformManager.TotalTime = EngineClock.TotalTime;
        UniformManager.DeltaTime = EngineClock.DeltaTime;
        UniformManager.CurrentFrame = EngineClock.CurrentFrame;

        // Matrices
        UniformManager.ViewMatrix = Renderer.Camera.GetViewMatrix();
        UniformManager.ProjectionMatrix = Renderer.Camera.GetProjectionMatrix();
        UniformManager.NormalMatrix = Renderer.Camera.GetNormalMatrix();
    }
}
