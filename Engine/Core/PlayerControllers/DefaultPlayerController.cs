using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Core.Controllers;

// TODO whem cursor is disabled, the ImGui still detects hovering and clicking
public class DefaultPlayerController : IPlayerController
{
    public DefaultPlayerController(AEngineBase engine)
    {
        _engine = engine;
        CursorEnabled = false;
    }

    private readonly AEngineBase _engine;

    private bool _cursorEnabled = false;
    public bool CursorEnabled
    {
        get => _cursorEnabled;
        set
        {
            _cursorEnabled = value;
            _engine.CursorState = _cursorEnabled ? CursorState.Normal : CursorState.Grabbed;
        }
    }

    public float MovementSpeed { get; set; } = 10.0f;
    public float MouseSensitivity { get; set; } = 0.1f;
    public float RunMultiplier { get; set; } = 2.5f;

    public void Update(float deltaTime)
    {
        if (!_engine.IsFocused) return;

        if (_engine.KeyboardState.IsKeyPressed(Keys.F1)) CursorEnabled = !CursorEnabled;
        if (_cursorEnabled) return;

        var camera = _engine.Renderer.Camera;

        var moveDir = Vector3.Zero;
        if (_engine.KeyboardState.IsKeyDown(Keys.W)) moveDir += camera.Front;
        if (_engine.KeyboardState.IsKeyDown(Keys.S)) moveDir -= camera.Front;
        if (_engine.KeyboardState.IsKeyDown(Keys.A)) moveDir -= camera.Right;
        if (_engine.KeyboardState.IsKeyDown(Keys.D)) moveDir += camera.Right;
        if (_engine.KeyboardState.IsKeyDown(Keys.Space)) moveDir += Vector3.UnitY;
        if (_engine.KeyboardState.IsKeyDown(Keys.LeftControl)) moveDir -= Vector3.UnitY;

        if (moveDir != Vector3.Zero)
        {
            var speed = MovementSpeed;
            if (_engine.KeyboardState.IsKeyDown(Keys.LeftShift)) speed *= RunMultiplier;

            camera.Position += moveDir.Normalized() * speed * deltaTime;
        }

        if (_engine.MouseState.Delta != Vector2.Zero)
        {
            camera.Yaw += _engine.MouseState.Delta.X * MouseSensitivity;
            camera.Pitch -= _engine.MouseState.Delta.Y * MouseSensitivity;
        }
    }

}
