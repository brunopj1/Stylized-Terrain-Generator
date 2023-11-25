using Engine.Graphics;
using ImGuiNET;

namespace Engine.Util.SmartProperties;

public class PropertyGroup : ICustomUniformManager
{
    public PropertyGroup(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    private readonly List<IProperty> _properties = new();

    internal void AddProperty(IProperty property)
    {
        _properties.Add(property);
    }

    public void BindUniforms(AShader shader)
    {
        foreach (var property in _properties)
        {
            if (property.HasShaderUniform)
            {
                property.BindUniform(shader);
            }
        }
    }

    public bool RenderWindow()
    {
        ImGui.Begin(Name);

        var updated = RenderProperties();

        ImGui.End();

        return updated;
    }

    public bool RenderTab()
    {
        if (!ImGui.BeginTabItem(Name)) return false;

        var updated = RenderProperties();

        ImGui.EndTabItem();

        return updated;
    }

    private bool RenderProperties()
    {
        var updated = false;

        foreach (var property in _properties)
        {
            if (property.HasInputField)
            {
                updated = property.RenderInputField() || updated;
            }
        }

        return updated;
    }
}
