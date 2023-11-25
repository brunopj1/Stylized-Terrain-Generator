﻿using Engine.Extensions;
using Engine.Graphics;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class Color3Property : AProperty<Vector3>
{
    public Color3Property(PropertyGroup group, string name, Vector3 value)
        : base(group, name, value)
    {
    }

    protected override void ApplyValueSettings()
    {
        var x = MathHelper.Clamp(_value.X, 0.0f, 1.0f);
        var y = MathHelper.Clamp(_value.Y, 0.0f, 1.0f);
        var z = MathHelper.Clamp(_value.Z, 0.0f, 1.0f);
        _value = new(x, y, z);
    }

    public override void BindUniform(AShader shader)
    {
        shader.BindUniform(_uniformName, _value);
    }

    public override bool RenderInputField()
    {
        var tempValue = _value.ToNumerics();

        if (ImGui.ColorEdit3(_name, ref tempValue))
        {
            Value = tempValue.ToOpenTK();
            return true;
        }

        return false;
    }
}
