namespace Engine.Graphics;
public class TextureUniform
{
    public TextureUniform(Texture texture, string name)
    {
        Texture = texture;
        Name = name;
    }

    public Texture Texture { get; set; }
    public string Name { get; set; }

}
