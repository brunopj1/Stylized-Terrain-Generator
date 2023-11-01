using Engine.Core.Services;
using OpenTK.Mathematics;

namespace Engine.Graphics;

public class Model<M> where M : struct
{
    public Model(Mesh<M> mesh, Shader shader)
    {
        Mesh = mesh;
        Shader = shader;
        Transform = new();
    }

    public Mesh<M> Mesh { get; set; }
    public Shader Shader { get; set; }
    public Transform Transform { get; set; }

    public void Render(UniformManager uniformManager, Matrix4? parentModelMatrix = null)
    {
        var modelMatrix = Transform.GetMatrix();
        if (parentModelMatrix.HasValue) modelMatrix = parentModelMatrix.Value * modelMatrix;

        Shader.Use();
        Shader.BindUniforms(uniformManager, modelMatrix);
        Mesh.Render();
    }
}
