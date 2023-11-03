using Engine.Core.Services;
using OpenTK.Mathematics;

namespace Engine.Graphics;

public class Model
{
    internal Model(Mesh mesh, Shader shader, BoundingVolume? boundingVolume = null)
    {
        Mesh = mesh;
        Shader = shader;
        Transform = new();
        BoundingVolume = boundingVolume;
    }

    public Mesh Mesh { get; set; }
    public Shader Shader { get; set; }
    public List<TextureUniform> Textures { get; set; } = new();
    public Transform Transform { get; set; }
    public BoundingVolume? BoundingVolume { get; set; }

    public void Render(Camera camera, UniformManager uniformManager, Matrix4? parentModelMatrix = null)
    {
        var modelMatrix = Transform.GetModelMatrix();
        if (parentModelMatrix.HasValue) modelMatrix = parentModelMatrix.Value * modelMatrix;

        Console.WriteLine($"Rendering: {camera.Intersect(BoundingVolume!, modelMatrix)}");
        if (BoundingVolume != null && !camera.Intersect(BoundingVolume, modelMatrix)) return;

        Shader.Use();
        Shader.BindUniforms(uniformManager, Textures, modelMatrix);
        Mesh.Render();
    }
}
