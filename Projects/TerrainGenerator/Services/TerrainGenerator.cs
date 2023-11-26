using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Services;
using Engine.Graphics;
using Engine.Util.SmartProperties;
using Engine.Util.SmartProperties.Properties;
using Engine.Util.SmartProperties.Properties.Other;
using ImGuiNET;

namespace TerrainGenerator.Services;
internal class TerrainGenerator
{
    public TerrainGenerator()
    {
        // Plains

        _plainsPropertyGroup = new PropertyGroup("Plains");

        new FloatProperty(_plainsPropertyGroup, "Plains Ground Pattern Freq", 2f);
        new FloatProperty(_plainsPropertyGroup, "Plains Height Pattern Freq", 5f);

        new PropertySeparator(_plainsPropertyGroup);

        new Color3Property(_plainsPropertyGroup, "Plains Grass Color 0", new(0.183f, 0.539f, 0.111f));
        new Color3Property(_plainsPropertyGroup, "Plains Grass Color 1", new(0.141f, 0.662f, 0.036f));
        new Color3Property(_plainsPropertyGroup, "Plains Grass Color 2", new(0.180f, 0.471f, 0.122f));
        new Color3Property(_plainsPropertyGroup, "Plains Grass Color 3", new(0.141f, 0.662f, 0.036f));

        new PropertySeparator(_plainsPropertyGroup);

        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 0", new(0.3725f, 0.2549f, 0.2196f));
        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 1", new(0.4392f, 0.3020f, 0.2588f));
        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 2", new(0.3176f, 0.2039f, 0.1804f));
        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 3", new(0.3725f, 0.2549f, 0.2196f));
        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 4", new(0.2510f, 0.1529f, 0.1333f));
        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 5", new(0.3176f, 0.2039f, 0.1804f));

        new Color3Property(_plainsPropertyGroup, "Plains Snow Color 0", new(0.9765f, 0.9765f, 0.9765f));
        new Color3Property(_plainsPropertyGroup, "Plains Snow Color 1", new(0.8745f, 0.8745f, 0.8745f));

    }

    private readonly PropertyGroup _plainsPropertyGroup;

    public void BindUniforms(AShader shader)
    {
        _plainsPropertyGroup.BindUniforms(shader);
    }

    public bool RenderBiomeSettingsWindow()
    {
        return _plainsPropertyGroup.RenderWindow();
    }
}
