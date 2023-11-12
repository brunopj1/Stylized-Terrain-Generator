using Engine.Graphics;

namespace Engine.Core.Services;
public class Renderer
{
    public Renderer(EngineUniformManager uniformManager)
    {
        _uniformManager = uniformManager;
    }

    private bool _isLoaded = false;

    private readonly EngineUniformManager _uniformManager;

    public Camera Camera { get; set; } = new();

    // TODO when destroying the graphics objects check if they are being used by a model
    private readonly List<AShader> _shaders = new();
    private readonly List<Texture> _textures = new();
    private readonly List<Mesh> _meshes = new();
    private readonly List<Model> _models = new();

    public RenderShader CreateRenderShader(string vertPath, string fragPath, string? tescPath = null, string? tesePath = null, string? geomPath = null)
    {
        var shader = new RenderShader(vertPath, tescPath, tesePath, geomPath, fragPath);
        if (_isLoaded) shader.Compile();
        _shaders.Add(shader);
        return shader;
    }

    public ComputeShader CreateComputeShader(string path)
    {
        var shader = new ComputeShader(path);
        if (_isLoaded) shader.Compile();
        _shaders.Add(shader);
        return shader;
    }

    public void DestroyShader(AShader shader)
    {
        shader.Dispose();
        _shaders.Remove(shader);
    }

    public Texture CreateTexture(string path, TextureParameters? parameters = null)
    {
        var texture = new Texture(path, parameters);
        if (_isLoaded) texture.Load();
        _textures.Add(texture);
        return texture;
    }
    public Texture CreateTexture(Vector2i size, TextureParameters? parameters = null)
    {
        var texture = new Texture(size, parameters);
        if (_isLoaded) texture.Load();
        _textures.Add(texture);
        return texture;
    }

    public void DestroyTexture(Texture texture)
    {
        texture.Dispose();
        _textures.Remove(texture);
    }

    public Mesh CreateMesh<T>(T[] vertices, VertexLayout layout, MeshParameters? parameters = null) where T : struct, IVertex
    {
        var mesh = new Mesh<T>(vertices, null, layout, parameters);
        if (_isLoaded) mesh.Build();
        _meshes.Add(mesh);
        return mesh;
    }

    public Mesh CreateMesh<T>(T[] vertices, uint[] indices, VertexLayout layout, MeshParameters? parameters = null) where T : struct, IVertex
    {
        var mesh = new Mesh<T>(vertices, indices, layout, parameters);
        if (_isLoaded) mesh.Build();
        _meshes.Add(mesh);
        return mesh;
    }

    public void DestroyMesh(Mesh mesh)
    {
        mesh.Dispose();
        _meshes.Remove(mesh);
    }

    public Model CreateModel(Mesh mesh, RenderShader shader, BoundingVolume? boundingVolume = null, ICustomUniformManager? customUniformManager = null)
    {
        var model = new Model(mesh, shader, boundingVolume, customUniformManager);
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

        foreach (var texture in _textures)
        {
            texture.Load();
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

    public long RenderAllModels()
    {
        var count = 0L;

        foreach (var model in _models)
        {
            count += model.Render(Camera, _uniformManager);
        }

        return count;
    }

    public void RecompileAllShaders()
    {
        foreach (var shader in _shaders)
        {
            shader.Compile();
        }
    }
}
