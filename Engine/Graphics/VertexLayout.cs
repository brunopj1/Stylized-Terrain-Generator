using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace Engine.Graphics;

public class VertexLayout
{
    public VertexAttribute[] Attributes { get; private set; }

    public VertexLayout(VertexAttribute[] attributes)
    {
        Attributes = attributes;
    }

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
