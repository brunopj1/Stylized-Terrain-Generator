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

    public void RenderWindow()
    {
        ImGui.Begin(Name);

        foreach (var property in _properties)
        {
            if (property.HasInputField)
            {
                property.RenderInputField();
            }
        }

        ImGui.End();
    }
}
