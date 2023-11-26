namespace Engine.Helpers;
public static partial class VectorHelper
{
    public static Vector2 ParseVector2(string value)
    {
        var components = ExtractVectorValues(value, 2);
        return new(float.Parse(components[0]), float.Parse(components[1]));
    }

    public static Vector3 ParseVector3(string value)
    {
        var components = ExtractVectorValues(value, 3);
        return new(float.Parse(components[0]), float.Parse(components[1]), float.Parse(components[2]));
    }

    public static Vector4 ParseVector4(string value)
    {
        var components = ExtractVectorValues(value, 4);
        return new(float.Parse(components[0]), float.Parse(components[1]), float.Parse(components[2]), float.Parse(components[3]));
    }

    public static Vector2i ParseVector2i(string value)
    {
        var components = ExtractVectorValues(value, 2);
        return new(int.Parse(components[0]), int.Parse(components[1]));
    }

    public static Vector3i ParseVector3i(string value)
    {
        var components = ExtractVectorValues(value, 3);
        return new(int.Parse(components[0]), int.Parse(components[1]), int.Parse(components[2]));
    }

    public static Vector4i ParseVector4i(string value)
    {
        var components = ExtractVectorValues(value, 4);
        return new(int.Parse(components[0]), int.Parse(components[1]), int.Parse(components[2]), int.Parse(components[3]));
    }

    private static string[] ExtractVectorValues(string value, int expectedCount)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        if (!value.StartsWith("(") || !value.EndsWith(")"))
        {
            throw new FormatException($"Invalid vector format: {value}");
        }

        var components = value.Trim('(', ')').Split("; ");
        if (components.Length != expectedCount)
        {
            throw new FormatException($"Invalid vector format: {value}");
        }

        return components;
    }
}
