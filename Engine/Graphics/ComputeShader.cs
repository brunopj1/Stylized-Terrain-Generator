namespace Engine.Graphics;
public class ComputeShader : AShader
{
    public ComputeShader(string path)
    {
        _path = path;
    }

    private readonly string _path;

    protected override void CompileInternal()
    {
        var shader = CompileShader(_path, ShaderType.ComputeShader);
        var shaderArray = new[] { shader };

        _handle = CreateProgram(shaderArray);

        DeleteShaders(_handle, shaderArray);
    }

    public void Dispatch(int groupsX, int groupsY, int groupsZ)
    {
        GL.UseProgram(_handle);
        GL.DispatchCompute(groupsX, groupsY, groupsZ);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
    }
    public void Dispatch(Vector3i groups) => Dispatch(groups.X, groups.Y, groups.Z);
}
