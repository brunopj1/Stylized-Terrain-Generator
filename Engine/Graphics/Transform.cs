using OpenTK.Mathematics;

namespace Engine.Graphics;

public class Transform
{
    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
    }

    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }

    public void Translate(Vector3 translation)
    {
        Position += translation;
    }

    public void Rotate(Quaternion rotation)
    {
        Rotation *= rotation;
    }

    public void ScaleBy(Vector3 scale)
    {
        Scale *= scale;
    }

    public Matrix4 GetMatrix()
    {
        Matrix4 translationMatrix = Matrix4.CreateTranslation(Position);
        Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(Rotation);
        Matrix4 scaleMatrix = Matrix4.CreateScale(Scale);

        return scaleMatrix * rotationMatrix * translationMatrix;
    }
}