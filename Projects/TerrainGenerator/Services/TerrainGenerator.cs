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

        new PropertyPushItemWidth(_plainsPropertyGroup, 200);

        new BoolProperty(_plainsPropertyGroup, "Plains Enable Flowers");
        new FloatProperty(_plainsPropertyGroup, "Plains Flowers Patch Size");
        new FloatProperty(_plainsPropertyGroup, "Plains Flowers Threshold");
        new Color3Property(_plainsPropertyGroup, "Plains Flowers Color");

        new PropertySeparator(_plainsPropertyGroup);

        new FloatProperty(_plainsPropertyGroup, "Plains Ground Pattern Freq");
        new FloatProperty(_plainsPropertyGroup, "Plains Height Pattern Freq");

        new Color3Property(_plainsPropertyGroup, "Plains Grass Color 0");
        new Color3Property(_plainsPropertyGroup, "Plains Grass Color 1");
        new Color3Property(_plainsPropertyGroup, "Plains Grass Color 2");
        new Color3Property(_plainsPropertyGroup, "Plains Grass Color 3");

        new PropertySeparator(_plainsPropertyGroup);

        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 0");
        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 1");
        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 2");
        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 3");
        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 4");
        new Color3Property(_plainsPropertyGroup, "Plains Dirt Color 5");

        new Color3Property(_plainsPropertyGroup, "Plains Snow Color 0");
        new Color3Property(_plainsPropertyGroup, "Plains Snow Color 1");

        new PropertySeparator(_plainsPropertyGroup);
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
