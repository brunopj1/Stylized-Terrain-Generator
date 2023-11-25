﻿using Engine.Extensions;
using Engine.Graphics;
using Engine.Util.SmartProperties.Settings;
using ImGuiNET;

namespace Engine.Util.SmartProperties.Properties;

public class Vector4Property : AProperty<Vector4>
{
    public Vector4Property(PropertyGroup group, string name, Vector4 value)
        : base(group, name, value)
    {
    }

    public FloatPropertyRange Range { get; set; } = new();
    public FloatPropertyRenderSettings RenderSettings { get; set; } = new();

    protected override void ApplyValueSettings()
    {
        var x = MathHelper.Clamp(_value.X, Range.Min, Range.Max);
        var y = MathHelper.Clamp(_value.Y, Range.Min, Range.Max);
        var z = MathHelper.Clamp(_value.Z, Range.Min, Range.Max);
        var w = MathHelper.Clamp(_value.W, Range.Min, Range.Max);
        _value = new(x, y, z, w);
    }

    public override void BindUniform(AShader shader)
    {
        shader.BindUniform(_uniformName, _value);
    }

    public override bool RenderInputField()
    {
        var tempValue = _value.ToNumerics();

        if (RenderSettings.EnableDrag)
        {
            if (ImGui.DragFloat4(_name, ref tempValue, RenderSettings.DragStep, Range.Min, Range.Max, RenderSettings.Format))
            {
                Value = tempValue.ToOpenTK();
                return true;
            }

            return false;
        }
        else
        {
            if (ImGui.InputFloat4(_name, ref tempValue, RenderSettings.Format))
            {
                Value = tempValue.ToOpenTK();
                return true;
            }

            return false;
        }
    }
}
