namespace Engine.Graphics;

public class VertexLayout
{
    public VertexLayout(VertexAttribute[] attributes)
    {
        _attributes = attributes;
    }

    private VertexAttribute[] _attributes;

    public IEnumerable<VertexAttribute> Attributes => _attributes;

    public int GetVertexSize()
    {
        int stride = 0;
        foreach (var attr in Attributes)
        {
            stride += attr.Stride;
        }
        return stride;
    }
}