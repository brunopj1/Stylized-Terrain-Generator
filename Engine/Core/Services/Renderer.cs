using Engine.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Engine.Core.Services;
public class Renderer
{
    public Renderer(UniformManager uniformManager)
    {
        _uniformManager = uniformManager;
    }

    private bool _isLoaded = false;

    private readonly UniformManager _uniformManager;

    public Camera Camera { get; set; } = new();
    private readonly List<Shader> _shaders = new();
    private readonly List<Mesh> _meshes = new();
    private readonly List<Model> _models = new();

    public Shader CreateShader(string vertexShaderPath, string fragmentShaderPath)
    {
        var shader = new Shader(vertexShaderPath, fragmentShaderPath);
        if (_isLoaded) shader.Compile();
        _shaders.Add(shader);
        return shader;
    }

    public void DestroyShader(Shader shader)
    {
        shader.Dispose();
        _shaders.Remove(shader);
    }

    public Mesh CreateMesh<T>(T[] vertices, VertexLayout layout, int vertexCount = 0, PrimitiveType primitiveType = PrimitiveType.Triangles, BufferUsageHint usageHint = BufferUsageHint.StaticDraw) where T : struct
    {
        var mesh = new Mesh<T>(vertices, vertexCount, layout, primitiveType, usageHint);
        if (_isLoaded) mesh.Build();
        _meshes.Add(mesh);
        return mesh;
    }

    public void DestroyMesh(Mesh mesh)
    {
        mesh.Dispose();
        _meshes.Remove(mesh);
    }

    public Model CreateModel(Mesh mesh, Shader shader)
    {
        var model = new Model(mesh, shader);
        _models.Add(model);
        return model;
    }

    public void DestroyModel(Model model)
    {
        _models.Remove(model);
    }

    internal void Load()
    {
        foreach (var shader in _shaders)
        {
            shader.Compile();
        }

        foreach (var mesh in _meshes)
        {
            mesh.Build();
        }

        _isLoaded = true;
    }

    internal void Unload()
    {
        foreach (var shader in _shaders)
        {
            shader.Dispose();
        }
        _shaders.Clear();
    }

    public void RenderAll()
    {
        foreach (var model in _models)
        {
            model.Render(_uniformManager);
        }
    }
}
