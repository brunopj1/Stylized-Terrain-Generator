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
        // General

        _generalPropertyGroup = new PropertyGroup("General");

        new PropertyPushItemWidth(_generalPropertyGroup, 200);

        new FloatProperty(_generalPropertyGroup, "Chunk Height Step")
        {
            Range = new() { Min = 0.1f, Max = 100 },
            RenderSettings = new() { DragStep = 0.1f }
        };

        new FloatProperty(_generalPropertyGroup, "Edge Distance")
        {
            Range = new() { Min = 0, Max = 0.34f }
        };

        new PropertyPopItemWidth(_generalPropertyGroup);

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

        new PropertyPopItemWidth(_plainsPropertyGroup);
    }

    private readonly PropertyGroup _generalPropertyGroup;

    private readonly PropertyGroup _plainsPropertyGroup;

    public void BindRenderUniforms(AShader shader)
    {
        _generalPropertyGroup.BindUniforms(shader);
    }

    public void BindGenerationUniforms(AShader shader)
    {
        _generalPropertyGroup.BindUniforms(shader);
        _plainsPropertyGroup.BindUniforms(shader);
    }

    public bool RenderBiomeSettingsWindow()
    {
        bool updated = false;

        ImGui.Begin("Terrain Generation Settings");

        if (ImGui.BeginTabBar("Terrain Generation Settings Tabs"))
        {
            _ = _generalPropertyGroup.RenderAsTab(); // Frag only. Doesn't require texture update

            updated = _plainsPropertyGroup.RenderAsTab() || updated;

            ImGui.EndTabBar();
        }

        ImGui.End();

        return updated;
    }
}
