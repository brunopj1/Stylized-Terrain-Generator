using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Services.Uniforms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine.Graphics;

public class Model<M> where M : struct
{
    public Mesh<M> Mesh { get; set; }
    public Shader Shader { get; set; }
    public Transform Transform { get; set; }

    public Model(Mesh<M> mesh, Shader shader)
    {
        Mesh = mesh;
        Shader = shader;
        Transform = new();
    }

    public void Render(IUniformAccessor uniformAccessor, Matrix4? parentModelMatrix = null)
    {
        var modelMatrix = Transform.GetMatrix();
        if (parentModelMatrix.HasValue) modelMatrix = parentModelMatrix.Value * modelMatrix;

        Shader.Use();
        Shader.BindUniforms(uniformAccessor, modelMatrix);
        Mesh.Render();
    }
}
