﻿using Engine.Core;
using Engine.Graphics;
using Engine.Util.ObjMesh;
using Engine.Util.PlayerControllers;
using OpenTK.Windowing.Common;
using TerrainGenerator.Vertices;

namespace Test;

internal class TestEngine : AEngineBase
{
    public TestEngine()
        : base()
    {
        Size = new(1600, 900);
        ClientLocation = new(50, 50);
        Title = "Test";
        ClearColor = new(0.2f, 0.3f, 0.3f);

        // Camera
        Renderer.Camera.Position = new(0.0f, 0.0f, 50.0f);

        // Player Controller
        PlayerController = new FlyingPlayerController(this);

        // Shaders
        var shader1 = Renderer.CreateRenderShader
        (
            @"Assets\Shaders\triangle.vert",
            @"Assets\Shaders\triangle.frag"
        );

        var shader2 = Renderer.CreateRenderShader
        (
            @"Assets\Shaders\square.vert",
            @"Assets\Shaders\square.frag"
        );

        var shader3 = Renderer.CreateRenderShader
        (
            @"Assets\Shaders\box.vert",
            @"Assets\Shaders\box.frag"
        );

        // Textures
        var boxTexture = Renderer.CreateTexture(@"Assets\Textures\box.jpg");
        var smileTexture = Renderer.CreateTexture(@"Assets\Textures\smile.png");
        _computedTexture = Renderer.CreateTexture(new Vector2i(100, 100), new() { MinFilter = TextureMinFilter.Nearest, MagFilter = TextureMagFilter.Nearest });

        // Axis
        var axisVertices = new TriangleVertex[]
        {
            new TriangleVertex{ Position = new( 0f, 0f, 0f), Color = new(1f, 0f, 0f) },
            new TriangleVertex{ Position = new( 1f, 0f, 0f), Color = new(1f, 0f, 0f) },
            new TriangleVertex{ Position = new( 0f, 0f, 0f), Color = new(0f, 1f, 0f) },
            new TriangleVertex{ Position = new( 0f, 1f, 0f), Color = new(0f, 1f, 0f) },
            new TriangleVertex{ Position = new( 0f, 0f, 0f), Color = new(0f, 0f, 1f) },
            new TriangleVertex{ Position = new( 0f, 0f, 1f), Color = new(0f, 0f, 1f) }
        };
        var axisMesh = Renderer.CreateMesh(axisVertices, TriangleVertex.GetLayout(), new() { PrimitiveType = PrimitiveType.Lines });
        var axisModel = Renderer.CreateModel(axisMesh, shader1);
        axisModel.BoundingVolume = new BoundingSphere(new(0.5f, 0.5f, 0.5f), 0.5f);

        // Triangle
        var triangleVertices = new TriangleVertex[]
        {
            new TriangleVertex{ Position = new( -1f, -1f, 0f), Color = new(1f, 0f, 0f) },
            new TriangleVertex{ Position = new(  1f, -1f, 0f), Color = new(0f, 1f, 0f) },
            new TriangleVertex{ Position = new(  0f,  1f, 0f), Color = new(0f, 0f, 1f) }
        };
        var triangleMesh = Renderer.CreateMesh(triangleVertices, TriangleVertex.GetLayout());
        var triangleModel = Renderer.CreateModel(triangleMesh, shader1);
        triangleModel.CumputeDefaultBoundingVolume();
        triangleModel.Transform.Position = new(15, 0, 0);
        triangleModel.Transform.Scale = new(10);

        // Square
        var squareVertices = new BoxVertex[]
        {
            new BoxVertex{ Position = new( -1f, -1f, 0f), TexCoord = new(0f, 0f) },
            new BoxVertex{ Position = new(  1f, -1f, 0f), TexCoord = new(1f, 0f) },
            new BoxVertex{ Position = new( -1f,  1f, 0f), TexCoord = new(0f, 1f) },
            new BoxVertex{ Position = new(  1f,  1f, 0f), TexCoord = new(1f, 1f) }
        };
        var squareIndices = new uint[] { 0, 1, 2, 1, 3, 2 };
        var squareMesh = Renderer.CreateMesh(squareVertices, squareIndices, BoxVertex.GetLayout());
        var squareModel = Renderer.CreateModel(squareMesh, shader2);
        squareModel.CumputeDefaultBoundingVolume();
        squareModel.Textures.Add(new(_computedTexture, "texture0"));
        squareModel.Textures.Add(new(smileTexture, "texture1"));
        squareModel.Transform.Position = new(-15, 0, 0);
        squareModel.Transform.Scale = new(10);

        // Box
        var boxMesh = ObjParser.Parse(@"Assets\Meshes\cube.obj", Renderer);
        var boxModel = Renderer.CreateModel(boxMesh, shader3);
        boxModel.CumputeDefaultBoundingVolume();
        boxModel.Textures.Add(new(boxTexture, "texture0"));
        boxModel.Transform.Position = new(-40, 0, -10);
        boxModel.Transform.Scale = new(10);
    }

    private readonly Texture _computedTexture;

    protected override void OnLoad()
    {
        base.OnLoad();

        VSync = VSyncMode.On;

        // Compute shader
        var computeShader = Renderer.CreateComputeShader(@"Assets\Shaders\texture.comp");
        computeShader.Use();
        computeShader.BindUniform("texture0", _computedTexture, 0, TextureAccess.WriteOnly);
        computeShader.Dispatch(100, 100, 1);
    }
}
