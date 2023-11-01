namespace Engine.Graphics;

public class VertexLayout
{
    public VertexLayout(VertexAttribute[] attributes)
    {
        _attributes = attributes;
    }

    private readonly VertexAttribute[] _attributes;

    public IEnumerable<VertexAttribute> Attributes => _attributes;

    public int GetVertexSize()
    {
        var stride = 0;
        foreach (var attr in Attributes)
        {
            stride += attr.Stride;
        }
        return stride;
    }
}
