using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Services;
using Engine.Graphics;
using Engine.Util.SmartProperties;
using Engine.Util.SmartProperties.Properties;
using ImGuiNET;

namespace TerrainGenerator.Services;
internal class TerrainGenerator
{
    public TerrainGenerator()
    {
        // Plains

        _plainsPropertyGroup = new PropertyGroup("Plains");

        PlainsColorGrass0 = new(_plainsPropertyGroup, "Plains Color Grass 0", new(0.112f, 0.630f, 0.007f));
        PlainsColorGrass1 = new(_plainsPropertyGroup, "Plains Color Grass 1", new(0.219f, 0.736f, 0.114f));
        PlainsColorGrass2 = new(_plainsPropertyGroup, "Plains Color Grass 2", new(0.322f, 0.862f, 0.212f));
        PlainsColorGrass3 = new(_plainsPropertyGroup, "Plains Color Grass 3", new(0.112f, 0.630f, 0.007f));
        PlainsColorGrass4 = new(_plainsPropertyGroup, "Plains Color Grass 4", new(0.175f, 0.514f, 0.107f));
        PlainsColorGrass5 = new(_plainsPropertyGroup, "Plains Color Grass 5", new(0.170f, 0.633f, 0.076f));

        PlainsColorDirt0 = new(_plainsPropertyGroup, "Plains Color Dirt 0", new(0.3725f, 0.2549f, 0.2196f));
        PlainsColorDirt1 = new(_plainsPropertyGroup, "Plains Color Dirt 1", new(0.4392f, 0.3020f, 0.2588f));
        PlainsColorDirt2 = new(_plainsPropertyGroup, "Plains Color Dirt 2", new(0.3176f, 0.2039f, 0.1804f));
        PlainsColorDirt3 = new(_plainsPropertyGroup, "Plains Color Dirt 3", new(0.3725f, 0.2549f, 0.2196f));
        PlainsColorDirt4 = new(_plainsPropertyGroup, "Plains Color Dirt 4", new(0.2510f, 0.1529f, 0.1333f));
        PlainsColorDirt5 = new(_plainsPropertyGroup, "Plains Color Dirt 5", new(0.3176f, 0.2039f, 0.1804f));

        PlainsColorSnow0 = new(_plainsPropertyGroup, "Plains Color Snow 0", new(0.9765f, 0.9765f, 0.9765f));
        PlainsColorSnow1 = new(_plainsPropertyGroup, "Plains Color Snow 1", new(0.8745f, 0.8745f, 0.8745f));

    }

    private readonly PropertyGroup _plainsPropertyGroup;

    public Color3Property PlainsColorGrass0 { get; }
    public Color3Property PlainsColorGrass1 { get; }
    public Color3Property PlainsColorGrass2 { get; }
    public Color3Property PlainsColorGrass3 { get; }
    public Color3Property PlainsColorGrass4 { get; }
    public Color3Property PlainsColorGrass5 { get; }

    public Color3Property PlainsColorDirt0 { get; }
    public Color3Property PlainsColorDirt1 { get; }
    public Color3Property PlainsColorDirt2 { get; }
    public Color3Property PlainsColorDirt3 { get; }
    public Color3Property PlainsColorDirt4 { get; }
    public Color3Property PlainsColorDirt5 { get; }

    public Color3Property PlainsColorSnow0 { get; }
    public Color3Property PlainsColorSnow1 { get; }

    public void BindUniforms(AShader shader)
    {
        _plainsPropertyGroup.BindUniforms(shader);
    }

    public bool RenderBiomeSettingsWindow()
    {
        return _plainsPropertyGroup.RenderWindow();
    }
}
