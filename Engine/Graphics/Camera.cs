using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics;

public class Camera
{
    public Vector3 Position { get; set; } = Vector3.Zero;

    private float _pitch = 0; // Rotation around the X axis (radians)
    private float _yaw = -MathHelper.PiOver2; // Rotation around the Y axis (radians)

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

    private float _fov = MathHelper.PiOver2;

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }

    private float _near = 0.1f;
    private float _far = 100f;

    public float Near
    {
        get => _near;
        set
        {
            if (value <= 0f) throw new ArgumentOutOfRangeException(nameof(value), "Near plane must be greater than 0.");
            if (value >= _far) throw new ArgumentOutOfRangeException(nameof(value), "Near plane must be less than far plane.");
            _near = value;
    }
    }

    public float Far
    {
        get => _far;
        set
        {
            if (value <= 0f) throw new ArgumentOutOfRangeException(nameof(value), "Far plane must be greater than 0.");
            if (value <= _near) throw new ArgumentOutOfRangeException(nameof(value), "Far plane must be greater than near plane.");
            _far = value;
        }
    }

    public float AspectRatio { private get; set; } = 1;

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
}
