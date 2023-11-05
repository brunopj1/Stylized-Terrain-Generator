using Engine.Core.Services;

namespace Engine.Graphics;

public class Model
{
    internal Model(Mesh mesh, Shader shader, BoundingVolume? boundingVolume = null, ICustomUniformManager? customUniformManager = null)
    {
        Mesh = mesh;
        Shader = shader;
        Transform = new();
        Textures = new();
        BoundingVolume = boundingVolume;
        CustomUniformManager = customUniformManager;
    }

    public Mesh Mesh { get; set; }
    public Shader Shader { get; set; }
    public List<TextureUniform> Textures { get; set; }
    public Transform Transform { get; set; }
    public BoundingVolume? BoundingVolume { get; set; }
    public ICustomUniformManager? CustomUniformManager { get; set; }

    public void CumputeDefaultBoundingVolume()
    {
        BoundingVolume = Mesh.ComputeBoundingVolume();
    }

    public void Render(Camera camera, EngineUniformManager engineUniformManager, Matrix4? parentModelMatrix = null)
    {
        var modelMatrix = Transform.GetModelMatrix();
        if (parentModelMatrix.HasValue) modelMatrix = parentModelMatrix.Value * modelMatrix;

        if (BoundingVolume != null && !camera.Intersect(BoundingVolume, modelMatrix)) return;

        Shader.Use();

        for (var i = 0; i < Textures.Count; i++)
        {
            var textureUniform = Textures[i];
            textureUniform.Texture.Bind(i);
            Shader.BindUniform(textureUniform.Name, i);
        }

        engineUniformManager.BindUniforms(Shader, ref modelMatrix);
        CustomUniformManager?.BindUniforms(Shader);

        Mesh.Render();
    }
}
