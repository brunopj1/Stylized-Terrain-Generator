using Engine.Core.Services.Internal;

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

    public bool IsEnabled { get; set; } = true;

    public void CumputeDefaultBoundingVolume()
    {
        BoundingVolume = Mesh.ComputeBoundingVolume();
    }

    internal ulong Render(EngineUniformManager engineUniformManager, Matrix4? parentModelMatrix = null)
    {
        if (!IsEnabled) return 0;
        if (BoundingVolume?.IsVisible == false) return 0;

        var modelMatrix = Transform.ModelMatrix;
        if (parentModelMatrix.HasValue) modelMatrix = parentModelMatrix.Value * modelMatrix;

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
