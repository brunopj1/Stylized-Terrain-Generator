using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics;

// TODO should I validate the properties here?
public class Camera
{
    public Vector3 Position { get; set; } = new(0.0f, 0.0f, 0.0f);
    public Vector3 Front { get; set; } = new(0.0f, 0.0f, -1.0f);
    public Vector3 Up { get; set; } = new(0.0f, 1.0f, 0.0f);

    public float Fov { get; set; } = 45.0f;
    public float AspectRatio { get; set; } = 1.0f;

    public float Near { get; set; } = 0.1f;
    public float Far { get; set; } = 100.0f;

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + Front, Up);
    }

    public Matrix4 GetProjectionMatrix(float aspectRatio)
    {
        return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov), aspectRatio, Near, Far);
    }
}
