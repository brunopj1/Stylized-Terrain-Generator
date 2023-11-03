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

    public void TranslateBy(Vector3 translation)
    {
        Position += translation;
    }

    public void RotateBy(Quaternion rotation)
    {
        Rotation *= rotation;
    }

    public void ScaleBy(Vector3 scale)
    {
        Scale *= scale;
    }

    public Matrix4 GetModelMatrix()
    {
        var translationMatrix = Matrix4.CreateTranslation(Position);
        var rotationMatrix = Matrix4.CreateFromQuaternion(Rotation);
        var scaleMatrix = Matrix4.CreateScale(Scale);

        return scaleMatrix * rotationMatrix * translationMatrix;
    }
}
