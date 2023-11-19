using StbImageSharp;

namespace Engine.Graphics;

public struct TextureParameters
{
    public TextureParameters()
    {
    }

    public PixelInternalFormat PixelInternalFormat { get; set; } = PixelInternalFormat.Rgba32f;
    public PixelFormat PixelFormat { get; set; } = PixelFormat.Rgba;
    public PixelType PixelType { get; set; } = PixelType.UnsignedByte;

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
        _size = null;

        _params = parameters ?? new();
    }

    internal Texture(Vector2i size, TextureParameters? parameters)
    {
        _path = null;
        _size = size;

        _params = parameters ?? new();
    }

    private readonly string? _path;
    private readonly Vector2i? _size;

    private readonly TextureParameters _params;

    private int _handle = -1;
    internal int Handle => _handle;

    internal void Load()
    {
        if (_handle != -1) Dispose();

        _handle = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _handle);

        if (_path != null)
        {
            var image = ImageResult.FromStream(File.OpenRead(_path), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, _params.PixelInternalFormat, image.Width, image.Height, 0, _params.PixelFormat, _params.PixelType, image.Data);
        }
        else
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, _params.PixelInternalFormat, _size!.Value.X, _size!.Value.Y, 0, _params.PixelFormat, _params.PixelType, IntPtr.Zero);
        }

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)_params.MinFilter);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)_params.MagFilter);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)_params.WrapModeS);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)_params.WrapModeT);

        if (_params.Mipmap) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    internal void Dispose()
    {
        GL.DeleteTexture(_handle);
        _handle = -1;
    }

    internal void BindTexture(int unit)
    {
        if (_handle != -1)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }
    }

    internal void BindImageTexture(int unit, TextureAccess access)
    {
        if (_handle != -1)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindImageTexture(unit, _handle, 0, false, 0, access, (SizedInternalFormat)_params.PixelInternalFormat);
        }
    }
}
