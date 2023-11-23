using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Services;
using Engine.Extensions;
using Engine.Graphics;
using Engine.Util.EngineProperties;
using Engine.Util.EngineProperties.Properties;
using Engine.Util.EngineProperties.Properties.Other;
using Engine.Util.EngineProperties.Settings;
using Engine.Util.ObjMesh;
using ImGuiNET;
using TerrainGenerator.Graphics;

namespace TerrainGenerator.Services;

internal class SkyManager : ICustomUniformManager
{
    public SkyManager(Renderer renderer, ImGuiRenderer imGuiRenderer)
    {
        // Model

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

        // Properties

        imGuiRenderer.AddOverlay(_propertyGroup.RenderWindow);

        _propertyGroup.AddProperty(SkyColor0);
        _propertyGroup.AddProperty(SkyColor1);
        _propertyGroup.AddProperty(SkyNoiseFactor);
        _propertyGroup.AddProperty(SkyNoiseFreq);

        _propertyGroup.AddProperty(new PropertySeparator(2));

        _propertyGroup.AddProperty(CloudVoronoiNoiseFreq);
        _propertyGroup.AddProperty(CloudPerlinNoiseFreq);
        _propertyGroup.AddProperty(CloudThreshold);
        _propertyGroup.AddProperty(CloudExponent);
        _propertyGroup.AddProperty(CloudTimeFactor);
        _propertyGroup.AddProperty(CloudDirection);
    }

    private readonly Model _skyModel;

    private readonly PropertyGroup _propertyGroup = new("Sky Settings");

    public Color3Property SkyColor0 { get; set; } = new("Sky Color 0", new(0.23f, 0.43f, 0.71f));
    public Color3Property SkyColor1 { get; set; } = new("Sky Color 1", new(0.50f, 0.61f, 0.85f));
    public FloatProperty SkyNoiseFactor { get; set; } = new("Sky Noise Factor", 0.2f);
    public FloatProperty SkyNoiseFreq { get; set; } = new("Sky Noise Freq", 8f);

    public FloatProperty CloudVoronoiNoiseFreq { get; set; } = new("Cloud Voronoi Noise Freq", 16f);
    public FloatProperty CloudPerlinNoiseFreq { get; set; } = new("Cloud Perlin Noise Freq", 0.1f);
    public FloatProperty CloudThreshold { get; set; } = new("Cloud Threshold", 0.7f);
    public FloatProperty CloudExponent { get; set; } = new("Cloud Exponent", 0.9f);
    public FloatProperty CloudTimeFactor { get; set; } = new("Cloud Time Factor", 0.05f);
    public Vector3Property CloudDirection { get; set; } = new("Cloud Direction", new(0.8f, 0.2f, 0.1f));

    public void BindUniforms(AShader shader)
    {
        _propertyGroup.BindUniforms(shader);
    }
}
