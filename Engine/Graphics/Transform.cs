namespace Engine.Graphics;

public class Transform
{
    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
    }

    private Vector3 _position;
    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdateModelMatrix();
        }
    }

    private Quaternion _rotation;
    public Quaternion Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            UpdateModelMatrix();
        }
    }

    private Vector3 _scale;
    public Vector3 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            UpdateModelMatrix();
        }
    }

    private Matrix4 _modeLMatrix;
    public Matrix4 ModelMatrix
    {
        get => _modeLMatrix;
        private set => _modeLMatrix = value;
    }

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

    private void UpdateModelMatrix()
    {
        var translationMatrix = Matrix4.CreateTranslation(Position);
        var rotationMatrix = Matrix4.CreateFromQuaternion(Rotation);
        var scaleMatrix = Matrix4.CreateScale(Scale);

        _modeLMatrix = scaleMatrix * rotationMatrix * translationMatrix;
    }
}
