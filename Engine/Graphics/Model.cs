using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core;
using Engine.Core.Managers.Interfaces;
using OpenTK.Graphics.OpenGL4;

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

    public void Render()
    {
        Shader.Use();

        BindGeneralUniforms();

        Shader.BindCustomUniforms();

        Mesh.Render();
    }

    private void BindGeneralUniforms()
    {
        var shaderHandle = Shader.Handle;

        // Time
        ITimeManager timeManager = ITimeManager.Instance!;

        var uniformHandle = GL.GetUniformLocation(shaderHandle, "uTime");
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, (float)timeManager.TotalTime);

        uniformHandle = GL.GetUniformLocation(shaderHandle, "uDeltaTime");
        if (uniformHandle != -1) GL.Uniform1(uniformHandle, (float)timeManager.DeltaTime);

        // Matrices



        // uMatProj
        // uMatView
        // uMatModel
        // uMatNormal
        // uMatMVP
    }
}
