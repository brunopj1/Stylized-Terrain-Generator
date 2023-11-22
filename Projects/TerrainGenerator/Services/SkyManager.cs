using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Common.ObjMesh;
using Engine.Core.Services;
using Engine.Graphics;
using TerrainGenerator.Graphics;

namespace TerrainGenerator.Services;

internal class SkyManager
{
    public SkyManager(Renderer renderer, ImGuiRenderer imGuiRenderer)
    {
        var shader = renderer.CreateRenderShader
        (
            @"Assets/Shaders/skybox.vert",
            @"Assets/Shaders/skybox.frag"
        );

        var vertices = new SkyVertex[]
        {
            new() { Position = new(-1,  1) },
            new() { Position = new(-1, -1) },
            new() { Position = new( 1,  1) },

            new() { Position = new( 1, -1) },
            new() { Position = new( 1,  1) },
            new() { Position = new(-1, -1) }
        };

        var mesh = renderer.CreateMesh(vertices, SkyVertex.GetLayout());

        _skyModel = renderer.CreateModel(mesh, shader);
    }

    private readonly Model _skyModel;
}
