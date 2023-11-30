using System.Text;
using System.Xml;
using Engine.Graphics;
using ImGuiNET;

namespace Engine.Util.SmartProperties;

public class PropertyGroup : ICustomUniformManager
{
    public PropertyGroup(string name)
    {
        s_instances.Add(this);

        Name = name;
    }

    private static readonly List<PropertyGroup> s_instances = new();

    public string Name { get; set; }

    private readonly List<AProperty> _properties = new();

    internal void AddProperty(AProperty property)
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

    public bool RenderAsWindow()
    {
        ImGui.Begin(Name);

        var updated = RenderProperties();

        ImGui.End();

        return updated;
    }

    public bool RenderAsTab()
    {
        if (!ImGui.BeginTabItem(Name)) return false;

        var updated = RenderProperties();

        ImGui.EndTabItem();

        return updated;
    }

    public bool RenderProperties()
    {
        var updated = false;

        foreach (var property in _properties)
        {
            updated = property.RenderInputField() || updated;
        }

        return updated;
    }

    public static void SaveValuesToFile(string path)
    {
        var settings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = "\n",
            Encoding = Encoding.UTF8
        };

        using var writer = XmlWriter.Create(path, settings);

        writer.WriteStartDocument();
        writer.WriteStartElement("SmartProperties");

        foreach (var instance in s_instances)
        {
            writer.WriteStartElement("Group");
            writer.WriteAttributeString("Name", instance.Name);

            foreach (var property in instance._properties)
            {
                if (!property.AllowSerialization) continue;

                var valueStr = property.StringValue;
                if (valueStr == null) continue;

                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Name", property.Name);
                writer.WriteAttributeString("Value", valueStr);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
    }

    public static void LoadValuesFromFile(string path)
    {
        var document = new XmlDocument();
        document.Load(path);

        foreach (var instance in s_instances)
        {
            var groupNode = document.SelectSingleNode($"SmartProperties/Group[@Name='{instance.Name}']");
            if (groupNode == null) continue;

            foreach (var property in instance._properties)
            {
                if (!property.AllowSerialization) continue;

                var propertyNode = groupNode.SelectSingleNode($"Property[@Name='{property.Name}']");
                if (propertyNode == null) continue;

                var value = propertyNode?.Attributes?["Value"]?.Value;
                if (value == null) continue;

                property.StringValue = value;
            }
        }
    }
}
