using Engine.Extensions;

namespace Engine.Graphics;

public class Camera
{
    public Vector3 Position { get; set; } = Vector3.Zero;

    private float _pitch = 0;
    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            var angle = MathHelper.Clamp(value, -89f, 89f);
            _pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
            UpdateViewFrustum();
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
            UpdateViewFrustum();
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
            UpdateViewFrustum();
        }
    }

    private float _near = 0.1f;
    public float Near
    {
        get => _near;
        set
        {
            if (value <= 0f) throw new ArgumentOutOfRangeException(nameof(value), "Near plane must be greater than 0.");
            if (value >= _far) throw new ArgumentOutOfRangeException(nameof(value), "Near plane must be less than far plane.");
            _near = value;
            UpdateViewFrustum();
        }
    }

    private float _far = 100f;
    public float Far
    {
        get => _far;
        set
        {
            if (value <= 0f) throw new ArgumentOutOfRangeException(nameof(value), "Far plane must be greater than 0.");
            if (value <= _near) throw new ArgumentOutOfRangeException(nameof(value), "Far plane must be greater than near plane.");
            _far = value;
            UpdateViewFrustum();
        }
    }

    private float _aspectRatio = 1;
    public float AspectRatio
    {
        private get => _aspectRatio;
        set
        {
            _aspectRatio = value;
            UpdateViewFrustum();
        }
    }

    private readonly Vector4[] _viewFrustumPlanes = new Vector4[6];

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + Front, Up);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, _near, _far);
    }

    public Matrix4 GetNormalMatrix()
    {
        return Matrix4.Transpose(Matrix4.Invert(GetViewMatrix()));
    }

    public bool Intersect(BoundingVolume volume, Matrix4 modelMatrix)
    {
        return volume.IsOnFrustum(modelMatrix, _viewFrustumPlanes);
    }

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
    }

    private void UpdateViewFrustum()
    {
        var halfVSide = _far * MathF.Tan(_fov * 0.5f);
        var halfHSide = halfVSide * _aspectRatio;
        var frontMultFar = _far * Front;

        _viewFrustumPlanes[0] = ViewFrustumHelper.Plane(Position + _near * Front, Front);
        _viewFrustumPlanes[1] = ViewFrustumHelper.Plane(Position + frontMultFar, -Front);
        _viewFrustumPlanes[2] = ViewFrustumHelper.Plane(Position, Vector3.Cross(frontMultFar - Right * halfHSide, Up).Normalized());
        _viewFrustumPlanes[3] = ViewFrustumHelper.Plane(Position, Vector3.Cross(Up, frontMultFar + Right * halfHSide).Normalized());
        _viewFrustumPlanes[4] = ViewFrustumHelper.Plane(Position, Vector3.Cross(Right, frontMultFar - Up * halfVSide).Normalized());
        _viewFrustumPlanes[5] = ViewFrustumHelper.Plane(Position, Vector3.Cross(frontMultFar + Up * halfVSide, Right).Normalized());
    }
}
