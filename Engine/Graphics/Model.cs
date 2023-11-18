using Engine.Core.Services;

namespace Engine.Graphics;

public class Model
{
    internal Model(Mesh mesh, RenderShader shader, BoundingVolume? boundingVolume = null, ICustomUniformManager? customUniformManager = null)
    {
        Mesh = mesh;
        Shader = shader;
        Transform = new();
        Textures = new();
        BoundingVolume = boundingVolume;
        CustomUniformManager = customUniformManager;
    }

    public Mesh Mesh { get; set; }
    public RenderShader Shader { get; set; }
    public List<TextureUniform> Textures { get; set; }
    public Transform Transform { get; set; }
    public BoundingVolume? BoundingVolume { get; set; }
    public ICustomUniformManager? CustomUniformManager { get; set; }

    public void CumputeDefaultBoundingVolume()
    {
        BoundingVolume = Mesh.ComputeBoundingVolume();
    }

    public ulong Render(EngineUniformManager engineUniformManager, Matrix4? parentModelMatrix = null)
    {
        var modelMatrix = Transform.ModelMatrix;
        if (parentModelMatrix.HasValue) modelMatrix = parentModelMatrix.Value * modelMatrix;

        if (BoundingVolume?.IsVisible == false) return 0;

        Shader.Use();

        for (var i = 0; i < Textures.Count; i++)
        {
            var textureUniform = Textures[i];
            Shader.BindUniform(textureUniform.Name, textureUniform.Texture, i);
        }

        engineUniformManager.BindUniforms(Shader, ref modelMatrix);
        CustomUniformManager?.BindUniforms(Shader);

        return Mesh.Render();
    }
}
