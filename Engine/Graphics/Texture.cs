using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Engine.Graphics;

public struct TextureParameters
{
    public TextureParameters()
    {
    }

    public TextureMinFilter MinFilter { get; set; } = TextureMinFilter.LinearMipmapLinear;
    public TextureMagFilter MagFilter { get; set; } = TextureMagFilter.Linear;
    public TextureWrapMode WrapModeS { get; set; } = TextureWrapMode.Repeat;
    public TextureWrapMode WrapModeT { get; set; } = TextureWrapMode.Repeat;
    public bool Mipmap { get; set; } = true;
}

public class Texture
{
    internal Texture(string path, TextureParameters? parameters)
    {
        _path = path;
        _params = parameters ?? new TextureParameters();
    }

    private readonly string _path;
    private readonly TextureParameters _params;

    private int _id = -1;

    internal void Load()
    {
        _id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _id);

        var image = ImageResult.FromStream(File.OpenRead(_path), ColorComponents.RedGreenBlueAlpha);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)_params.MinFilter);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)_params.MagFilter);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)_params.WrapModeS);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)_params.WrapModeT);

        if (_params.Mipmap) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    internal void Dispose()
    {
        GL.DeleteTexture(_id);
        _id = -1;
    }

    public void Bind(int unit)
    {
        if (_id != -1)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(TextureTarget.Texture2D, _id);
        }
    }
}
