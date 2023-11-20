namespace Engine.Graphics;

public class RenderShader : AShader
{
    internal RenderShader(string vertPath, string? tesscPath, string? tessePath, string? geomPath, string fragPath)
    {
        _vertPath = vertPath;
        _tescPath = tesscPath;
        _tesePath = tessePath;
        _geomPath = geomPath;
        _fragPath = fragPath;
    }

    private readonly string? _vertPath;
    private readonly string? _tescPath;
    private readonly string? _tesePath;
    private readonly string? _geomPath;
    private readonly string? _fragPath;

    protected override void CompileInternal()
    {
        var shaders = new List<int>();

        if (_vertPath != null) shaders.Add(CompileShader(_vertPath, ShaderType.VertexShader));
        if (_tescPath != null) shaders.Add(CompileShader(_tescPath, ShaderType.TessControlShader));
        if (_tesePath != null) shaders.Add(CompileShader(_tesePath, ShaderType.TessEvaluationShader));
        if (_geomPath != null) shaders.Add(CompileShader(_geomPath, ShaderType.GeometryShader));
        if (_fragPath != null) shaders.Add(CompileShader(_fragPath, ShaderType.FragmentShader));

        _handle = CreateProgram(shaders);

        DeleteShaders(_handle, shaders);
    }
}
