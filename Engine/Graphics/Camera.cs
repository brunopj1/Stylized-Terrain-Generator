using Engine.Extensions;
using ImGuiNET;

namespace Engine.Graphics;

public class Camera
{
    private Vector3 _position = Vector3.Zero;
    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdateViewAndNormalMatrices();
            UpdateViewFrustum();
        }
    }

    private float _pitch = 0;
    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            var angle = MathHelper.Clamp(value, -89f, 89f);
            _pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    private float _yaw = -MathHelper.PiOver2;
    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            _yaw = MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    public Vector3 Front { get; private set; } = -Vector3.UnitZ;
    public Vector3 Up { get; private set; } = Vector3.UnitY;
    public Vector3 Right { get; private set; } = Vector3.UnitX;

    private float _fov = MathHelper.PiOver3;
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
            UpdateProjectionMatrix();
            UpdateViewFrustum();
        }
    }

    private float _near = 0.1f;
    public float Near
    {
        get => _near;
        set
        {
            _near = MathHelper.Clamp(value, 0.1f, _far);
            UpdateProjectionMatrix();
            UpdateViewFrustum();
        }
    }

    private float _far = 100f;
    public float Far
    {
        get => _far;
        set
        {
            _far = MathHelper.Clamp(value, _near + 0.1f, float.MaxValue);
            UpdateProjectionMatrix();
            UpdateViewFrustum();
        }
    }

    private float _aspectRatio = 1;
    public float AspectRatio
    {
        get => _aspectRatio;
        set
        {
            _aspectRatio = value;
            UpdateProjectionMatrix();
            UpdateViewFrustum();
        }
    }

    private readonly Vector4[] _viewFrustumPlanes = new Vector4[6];
    internal Vector4[] ViewFrustumPlanes => _viewFrustumPlanes;
    internal bool WasViewFrustumUpdated { get; set; } = true;

    public Matrix4 ViewMatrix { get; private set; }
    public Matrix4 ProjectionMatrix { get; private set; }
    public Matrix4 NormalMatrix { get; private set; }

    private void UpdateVectors()
    {
        Front = Vector3.Normalize(new Vector3
        {
            X = MathF.Cos(_pitch) * MathF.Cos(_yaw),
            Y = MathF.Sin(_pitch),
            Z = MathF.Cos(_pitch) * MathF.Sin(_yaw)
        });

        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));

        UpdateViewAndNormalMatrices();
        UpdateViewFrustum();
    }

    private void UpdateViewAndNormalMatrices()
    {
        ViewMatrix = Matrix4.LookAt(_position, _position + Front, Up);
        NormalMatrix = Matrix4.Transpose(Matrix4.Invert(ViewMatrix));
    }

    private void UpdateProjectionMatrix()
    {
        ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, _near, _far);
    }

    private void UpdateViewFrustum()
    {
        var halfVSide = _far * MathF.Tan(_fov * 0.5f);
        var halfHSide = halfVSide * _aspectRatio;
        var frontMultFar = _far * Front;

        _viewFrustumPlanes[0] = ViewFrustumHelper.Plane(_position + _near * Front, Front);
        _viewFrustumPlanes[1] = ViewFrustumHelper.Plane(_position + frontMultFar, -Front);
        _viewFrustumPlanes[2] = ViewFrustumHelper.Plane(_position, Vector3.Cross(frontMultFar - Right * halfHSide, Up).Normalized());
        _viewFrustumPlanes[3] = ViewFrustumHelper.Plane(_position, Vector3.Cross(Up, frontMultFar + Right * halfHSide).Normalized());
        _viewFrustumPlanes[4] = ViewFrustumHelper.Plane(_position, Vector3.Cross(Right, frontMultFar - Up * halfVSide).Normalized());
        _viewFrustumPlanes[5] = ViewFrustumHelper.Plane(_position, Vector3.Cross(frontMultFar + Up * halfVSide, Right).Normalized());

        WasViewFrustumUpdated = true;
    }

    internal void RenderCameraWindow()
    {
        ImGui.Begin("Camera Settings");

        System.Numerics.Vector3 tempV3 = new(_position.X, _position.Y, _position.Z);
        if (ImGui.DragFloat3("Position", ref tempV3)) Position = new(tempV3.X, tempV3.Y, tempV3.Z);

        var tempF = MathHelper.RadiansToDegrees(_pitch);
        if (ImGui.DragFloat("Pitch", ref tempF, 0.1f)) Pitch = tempF;

        tempF = MathHelper.RadiansToDegrees(_yaw);
        if (ImGui.DragFloat("Yaw", ref tempF, 0.1f)) Yaw = tempF;

        tempF = MathHelper.RadiansToDegrees(_fov);
        if (ImGui.DragFloat("Fov", ref tempF, 0.1f)) Fov = tempF;

        System.Numerics.Vector2 tempV2 = new(_near, _far);
        if (ImGui.DragFloat2("Near / Far", ref tempV2, 5f))
        {
            if (tempV2.X != _near) Near = tempV2.X;
            if (tempV2.Y != _far) Far = tempV2.Y;
        }

        ImGui.Separator();
        ImGui.Separator();

        tempV3 = new(Front.X, Front.Y, Front.Z);
        if (ImGui.InputFloat3("Front", ref tempV3, null, ImGuiInputTextFlags.ReadOnly)) Front = new(tempV3.X, tempV3.Y, tempV3.Z);

        ImGui.End();
    }
}
