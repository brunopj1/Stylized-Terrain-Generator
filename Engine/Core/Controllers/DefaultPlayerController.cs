using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Core.Controllers;
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

    public void Update(float deltaTime)
    {
        if (!_engine.IsFocused) return;

        if (_engine.KeyboardState.IsKeyPressed(Keys.F1)) CursorEnabled = !CursorEnabled;
        if (CursorEnabled) return;

        var moveDir = Vector3.Zero;
        if (_engine.KeyboardState.IsKeyDown(Keys.W)) moveDir += _engine.Camera.Front;
        if (_engine.KeyboardState.IsKeyDown(Keys.S)) moveDir -= _engine.Camera.Front;
        if (_engine.KeyboardState.IsKeyDown(Keys.A)) moveDir -= _engine.Camera.Right;
        if (_engine.KeyboardState.IsKeyDown(Keys.D)) moveDir += _engine.Camera.Right;
        if (_engine.KeyboardState.IsKeyDown(Keys.Space)) moveDir += Vector3.UnitY;
        if (_engine.KeyboardState.IsKeyDown(Keys.LeftControl)) moveDir -= Vector3.UnitY;

        if (moveDir != Vector3.Zero)
        {
            _engine.Camera.Position += moveDir.Normalized() * MovementSpeed * deltaTime;
        }

        if (_engine.MouseState.Delta != Vector2.Zero)
        {
            _engine.Camera.Yaw += _engine.MouseState.Delta.X * MouseSensitivity;
            _engine.Camera.Pitch -= _engine.MouseState.Delta.Y * MouseSensitivity;
        }
    }

}
