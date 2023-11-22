using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Common.ObjMesh;
using Engine.Core.Services;
using Engine.Extensions;
using Engine.Graphics;
using ImGuiNET;
using TerrainGenerator.Graphics;

namespace TerrainGenerator.Services;

internal class SkyManager : ICustomUniformManager
{
    public SkyManager(Renderer renderer, ImGuiRenderer imGuiRenderer)
    {
        var shader = renderer.CreateRenderShader
        (
            @"Assets/Shaders/skybox.vert",
            @"Assets/Shaders/skybox.frag"
        );

        var vertices = new SkyVertex[]
        {
            new() { Position = new(-1,  1) },
            new() { Position = new(-1, -1) },
            new() { Position = new( 1,  1) },

            new() { Position = new( 1, -1) },
            new() { Position = new( 1,  1) },
            new() { Position = new(-1, -1) }
        };

        var mesh = renderer.CreateMesh(vertices, SkyVertex.GetLayout());

        _skyModel = renderer.CreateModel(mesh, shader, customUniformManager: this);

        imGuiRenderer.AddOverlay(RenderSkySettingsWindow);
    }

    private readonly Model _skyModel;

    public Vector3 SkyColor0 { get; set; } = new(0.23f, 0.43f, 0.71f);
    public Vector3 SkyColor1 { get; set; } = new(0.08f, 0.20f, 0.48f);
    public float SkyNoiseFactor { get; set; } = 0.2f;
    public float SkyNoiseFreq { get; set; } = 8f;

    public float CloudVoronoiNoiseFreq { get; set; } = 16f;
    public float CloudPerlinNoiseFreq { get; set; } = 0.1f;
    public float CloudThreshold { get; set; } = 0.7f;
    public float CloudExponent { get; set; } = 0.9f;
    public float CloudTimeFactor { get; set; } = 0.05f;
    public Vector3 CloudDirection { get; set; } = new(0.8f, 0.2f, 0.1f);

    public void BindUniforms(AShader shader)
    {
        shader.BindUniform("uSkyColor0", SkyColor0);

        shader.BindUniform("uSkyColor1", SkyColor1);

        shader.BindUniform("uSkyNoiseFactor", SkyNoiseFactor);

        shader.BindUniform("uSkyNoiseFreq", SkyNoiseFreq);

        shader.BindUniform("uCloudVoronoiNoiseFreq", CloudVoronoiNoiseFreq);

        shader.BindUniform("uCloudPerlinNoiseFreq", CloudPerlinNoiseFreq);

        shader.BindUniform("uCloudThreshold", CloudThreshold);

        shader.BindUniform("uCloudExponent", CloudExponent);

        shader.BindUniform("uCloudTimeFactor", CloudTimeFactor);

        shader.BindUniform("uCloudDirection", CloudDirection);
    }

    private void RenderSkySettingsWindow()
    {
        ImGui.Begin("Sky Settings");

        var tempV3 = SkyColor0.ToNumerics();
        if (ImGui.ColorEdit3("Sky Color 0", ref tempV3)) SkyColor0 = tempV3.ToOpenTK();

        tempV3 = SkyColor1.ToNumerics();
        if (ImGui.ColorEdit3("Sky Color 1", ref tempV3)) SkyColor1 = tempV3.ToOpenTK();

        ImGui.PushItemWidth(100.0f);

        var tempF = SkyNoiseFactor;
        if (ImGui.DragFloat("Sky Noise Factor", ref tempF, 0.01f)) SkyNoiseFactor = tempF;

        tempF = SkyNoiseFreq;
        if (ImGui.DragFloat("Sky Noise Frequency", ref tempF, 0.01f)) SkyNoiseFreq = tempF;

        ImGui.Separator();
        ImGui.Separator();

        tempF = CloudVoronoiNoiseFreq;
        if (ImGui.DragFloat("Cloud Voronoi Noise Frequency", ref tempF, 0.01f)) CloudVoronoiNoiseFreq = tempF;

        tempF = CloudPerlinNoiseFreq;
        if (ImGui.DragFloat("Cloud Perlin Noise Frequency", ref tempF, 0.01f)) CloudPerlinNoiseFreq = tempF;

        tempF = CloudThreshold;
        if (ImGui.DragFloat("Cloud Threshold", ref tempF, 0.01f)) CloudThreshold = tempF;

        tempF = CloudExponent;
        if (ImGui.DragFloat("Cloud Exponent", ref tempF, 0.01f)) CloudExponent = tempF;

        tempF = CloudTimeFactor;
        if (ImGui.DragFloat("Cloud Time Factor", ref tempF, 0.01f)) CloudTimeFactor = tempF;

        ImGui.PopItemWidth();

        tempV3 = CloudDirection.ToNumerics();
        if (ImGui.DragFloat3("Cloud Direction", ref tempV3)) CloudDirection = tempV3.ToOpenTK();

        ImGui.End();
    }
}
