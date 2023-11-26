using Engine.Graphics;

namespace Engine.Util.SmartProperties;

public abstract class AProperty
{
    public AProperty(PropertyGroup group, string name)
    {
        group.AddProperty(this);

        Name = name;
        UniformName = $"u{Name.Replace(" ", "")}";
    }

    public string Name { get; }
    public string UniformName { get; }

    public abstract string StringValue { get; set; }

    public virtual bool AllowSerialization { get; set; } = true;
    public virtual bool HasShaderUniform { get; set; } = true;

    internal abstract void BindUniform(AShader shader);
    internal abstract bool RenderInputField();
}

public abstract class AProperty<T> : AProperty where T : struct
{
    public AProperty(PropertyGroup group, string name, T? value)
        : base(group, name)
    {
        Value = value ?? default;
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

    public delegate void ValueModifiedHandler(T oldValue, T newValue);
    public event ValueModifiedHandler? OnValueModified = null;

    protected abstract void ApplyValueSettings();

    internal override abstract void BindUniform(AShader shader);
    internal override abstract bool RenderInputField();
}
