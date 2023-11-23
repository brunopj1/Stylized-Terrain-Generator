using Engine.Graphics;

namespace Engine.Util.EngineProperties;

public interface IProperty
{
    void BindUniform(AShader shader);
    void RenderInputField();

    bool HasShaderUniform { get; set; }
    bool HasInputField { get; set; }
}

public abstract class AProperty<T> : IProperty where T : struct
{
    public AProperty(string name, T value)
    {
        Name = name;
        Value = value;
    }

    protected string _name = "";
    protected string _uniformName = "";

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            _uniformName = $"u{_name.Replace(" ", "")}";
        }
    }

    protected T _value = default;
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            ApplyValueSettings();
            OnValueModified?.Invoke(_value);
        }
    }

    public bool HasShaderUniform { get; set; } = true;
    public bool HasInputField { get; set; } = true;

    public delegate void ValueModifiedHandler(T newValue);
    public event ValueModifiedHandler? OnValueModified;

    protected abstract void ApplyValueSettings();

    public abstract void BindUniform(AShader shader);

    public abstract void RenderInputField();
}
