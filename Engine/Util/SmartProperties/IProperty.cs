using Engine.Graphics;

namespace Engine.Util.SmartProperties;

public interface IProperty
{
    void BindUniform(AShader shader);
    bool RenderInputField();

    bool HasShaderUniform { get; set; }
    bool HasInputField { get; set; }
}

public abstract class AProperty<T> : IProperty where T : struct
{
    public AProperty(PropertyGroup group, string name, T value)
    {
        group.AddProperty(this);

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
            var oldValue = _value;
            _value = value;
            ApplyValueSettings();
            OnValueModified?.Invoke(oldValue, _value);
        }
    }

    public bool HasShaderUniform { get; set; } = true;
    public bool HasInputField { get; set; } = true;

    public delegate void ValueModifiedHandler(T oldValue, T newValue);
    public event ValueModifiedHandler? OnValueModified = null;

    protected abstract void ApplyValueSettings();

    public abstract void BindUniform(AShader shader);

    public abstract bool RenderInputField();
}
