namespace Engine.Graphics;
public class ComputeShader : AShader
{
    public ComputeShader(string path)
    {
        _path = path;
    }

    private readonly string _path;

    public Vector3i GroupSize { get; set; } = new(1, 1, 1);

    public override void Compile()
    {
        base.Compile();

        var shader = CompileShader(_path, ShaderType.ComputeShader);
        var shaderArray = new[] { shader };

        _handle = CreateProgram(shaderArray);

        DeleteShaders(_handle, shaderArray);
    }

    public void Dispatch()
    {
        GL.UseProgram(_handle);
        GL.DispatchCompute(GroupSize.X, GroupSize.Y, GroupSize.Z);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
    }
}
