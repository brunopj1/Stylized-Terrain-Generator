using System.Runtime.InteropServices;
using Engine.Core.Controllers;
using Engine.Core.Services;
using Engine.Core.Services.Internal;
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
        : base(new(), new() { APIVersion = new(4, 6), Flags = ContextFlags.Debug })
    {
        if (s_wasAlreadyCreated) throw new MultipleEnginesException();
        else s_wasAlreadyCreated = true;

        EngineClock = new();
        UniformManager = new();
        Renderer = new(UniformManager);
        ImGuiRenderer = new();

        _imGuiOverlay = new(this, OnRecompileShaders);
    }

    // Singleton
    private static bool s_wasAlreadyCreated = false;

    // Settings
    public new string Title { get; set; } = "My Game";
    public Vector3 ClearColor { get; set; } = new(0.2f, 0.3f, 0.3f);

    // Information
    public ulong TriangleCount { get; private set; } = 0;

    // Public services
    public EngineClock EngineClock { get; private set; }
    public EngineUniformManager UniformManager { get; private set; }
    public Renderer Renderer { get; private set; }
    public ImGuiRenderer ImGuiRenderer { get; private set; }

    // Private services
    private ImGuiController? _imGuiController = null;
    private readonly EngineImGuiOverlay _imGuiOverlay;

    // Player controller
    public IPlayerController? PlayerController { get; set; } = null;

    // Debug
    private static void OnDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr pMessage, IntPtr pUserParam)
    {
        var message = Marshal.PtrToStringAnsi(pMessage, length);
        Console.WriteLine($"[{severity} source={source} type={type} id={id}] {message}");
        if (type == DebugType.DebugTypeError) throw new Exception(message);
    }

    public static void PrintOpenGLInfo()
    {
        Console.WriteLine("---------------------------------------------------");

        Console.WriteLine("OpenGL Version: " + GL.GetString(StringName.Version));
        Console.WriteLine("GLSL Version: " + GL.GetString(StringName.ShadingLanguageVersion));
        Console.WriteLine("Vendor: " + GL.GetString(StringName.Vendor));
        Console.WriteLine("Renderer: " + GL.GetString(StringName.Renderer));

        Console.WriteLine("---------------------------------------------------");
    }

    // Methods
    protected override void OnLoad()
    {
        base.OnLoad();

        PrintOpenGLInfo();

        GL.DebugMessageCallback(OnDebugMessage, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);

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
            base.Title = $"{Title} | {TriangleCount} triangles | {(int)ImGui.GetIO().Framerate} fps";
        }

        PlayerController?.Update((float)args.Time);

        _imGuiOverlay.ProcessInputs(KeyboardState);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override sealed void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        _imGuiController!.Update(this, (float)args.Time);

        UpdateUniforms();

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, 1.0f);

        Renderer.UpdateBoundingVolumes();
        TriangleCount = Renderer.Render() / 3;

        ImGuiRenderer.Render();
        _imGuiController!.Render();

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs args)
    {
        base.OnResize(args);

        GL.Viewport(0, 0, args.Width, args.Height);

        _imGuiController?.WindowResized(ClientSize.X, ClientSize.Y);

        Renderer.Camera.AspectRatio = (float)args.Width / args.Height;
    }

    protected virtual void OnRecompileShaders()
    {
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);

        _imGuiController!.PressChar((char)e.Unicode);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _imGuiController!.MouseScroll(e.Offset);
    }

    private void UpdateUniforms()
    {
        // Time
        UniformManager.TotalTime = EngineClock.TotalTime;
        UniformManager.DeltaTime = EngineClock.DeltaTime;
        UniformManager.CurrentFrame = EngineClock.CurrentFrame;

        // Matrices
        UniformManager.ViewMatrix = Renderer.Camera.ViewMatrix;
        UniformManager.ProjectionMatrix = Renderer.Camera.ProjectionMatrix;
        UniformManager.NormalMatrix = Renderer.Camera.NormalMatrix;
    }
}
