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
        imGuiRenderer.AddWindowOverlay(() => _propertyGroup.RenderWindow());

        new PropertyPushItemWidth(_propertyGroup, 200);

        new Vector3Property(_propertyGroup, "Sky Color 0");
        new Vector3Property(_propertyGroup, "Sky Color 1");
        new FloatProperty(_propertyGroup, "Sky Noise Factor");
        new FloatProperty(_propertyGroup, "Sky Noise Freq");

        new PropertySeparator(_propertyGroup, 2);

        new FloatProperty(_propertyGroup, "Cloud Voronoi Noise Freq");
        new FloatProperty(_propertyGroup, "Cloud Perlin Noise Freq");
        new FloatProperty(_propertyGroup, "Cloud Threshold");
        new FloatProperty(_propertyGroup, "Cloud Exponent");
        new FloatProperty(_propertyGroup, "Cloud Time Factor");
        new Vector3Property(_propertyGroup, "Cloud Direction");

        new PropertyPopItemWidth(_propertyGroup);
    }

    private readonly Model _skyModel;

    private readonly PropertyGroup _propertyGroup;

    public void BindUniforms(AShader shader)
    {
        _propertyGroup.BindUniforms(shader);
    }
}
