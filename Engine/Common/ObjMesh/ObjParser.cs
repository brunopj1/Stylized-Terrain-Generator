using Engine.Core.Services;
using Engine.Graphics;

namespace Engine.Common.ObjMesh;
public static class ObjParser
{
    public static Mesh Parse(string path, Renderer renderer)
    {
        var lines = File.ReadAllText(path).Split("\n");

        var positions = new List<Vector3>();
        var normals = new List<Vector3>();
        var texCoords = new List<Vector2>();
        var vertices = new List<ObjVertex>();

        foreach (var line in lines)
        {
            var components = line.Split(" ");
            if (components.Length == 0) continue;

            switch (components[0])
            {
                case "v":
                    positions.Add(new Vector3(float.Parse(components[1]), float.Parse(components[2]), float.Parse(components[3])));
                    break;
                case "vn":
                    normals.Add(new Vector3(float.Parse(components[1]), float.Parse(components[2]), float.Parse(components[3])));
                    break;
                case "vt":
                    texCoords.Add(new Vector2(float.Parse(components[1]), float.Parse(components[2])));
                    break;
                case "f":
                    vertices.Add(CreateVertex(components[1], positions, normals, texCoords));
                    vertices.Add(CreateVertex(components[2], positions, normals, texCoords));
                    vertices.Add(CreateVertex(components[3], positions, normals, texCoords));
                    break;
                default:
                    break;
            }
        }

        return renderer.CreateMesh(vertices.ToArray(), ObjVertex.GetLayout());
    }

    private static ObjVertex CreateVertex(string vertexStr, List<Vector3> positions, List<Vector3> normals, List<Vector2> texCoords)
    {
        var components = vertexStr.Split("/");

        return new ObjVertex
        {
            Position = positions[int.Parse(components[0]) - 1],
            TexCoord = texCoords[int.Parse(components[1]) - 1],
            Normal = normals[int.Parse(components[2]) - 1]
        };
    }
}
