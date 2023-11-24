using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Services;
using Engine.Extensions;
using Engine.Graphics;
using Engine.Util.ObjMesh;
using Engine.Util.SmartProperties;
using Engine.Util.SmartProperties.Properties;
using Engine.Util.SmartProperties.Properties.Other;
using Engine.Util.SmartProperties.Settings;
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

        // Smart Properties

        _propertyGroup = new("Sky Settings");
        imGuiRenderer.AddOverlay(_propertyGroup.RenderWindow);

        SkyColor0 = new(_propertyGroup, "Sky Color 0", new(0.23f, 0.43f, 0.71f));

        SkyColor1 = new(_propertyGroup, "Sky Color 1", new(0.50f, 0.61f, 0.85f));

        SkyNoiseFactor = new(_propertyGroup, "Sky Noise Factor", 0.2f);

        SkyNoiseFreq = new(_propertyGroup, "Sky Noise Freq", 8f);

        _ = new PropertySeparator(_propertyGroup, 2);

        CloudVoronoiNoiseFreq = new(_propertyGroup, "Cloud Voronoi Noise Freq", 16f);

        CloudPerlinNoiseFreq = new(_propertyGroup, "Cloud Perlin Noise Freq", 0.1f);

        CloudThreshold = new(_propertyGroup, "Cloud Threshold", 0.7f);

        CloudExponent = new(_propertyGroup, "Cloud Exponent", 0.9f);

        CloudTimeFactor = new(_propertyGroup, "Cloud Time Factor", 0.05f);

        CloudDirection = new(_propertyGroup, "Cloud Direction", new(0.8f, 0.2f, 0.1f));
    }

    private readonly Model _skyModel;

    private readonly PropertyGroup _propertyGroup;

    public Color3Property SkyColor0 { get; }
    public Color3Property SkyColor1 { get; }
    public FloatProperty SkyNoiseFactor { get; }
    public FloatProperty SkyNoiseFreq { get; }

    public FloatProperty CloudVoronoiNoiseFreq { get; }
    public FloatProperty CloudPerlinNoiseFreq { get; }
    public FloatProperty CloudThreshold { get; }
    public FloatProperty CloudExponent { get; }
    public FloatProperty CloudTimeFactor { get; }
    public Vector3Property CloudDirection { get; }

    public void BindUniforms(AShader shader)
    {
        _propertyGroup.BindUniforms(shader);
    }
}
