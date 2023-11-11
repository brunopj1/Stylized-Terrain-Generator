using ImGuiNET;

namespace Engine.Helpers;
public static class ImGuiHelper
{
    public static bool InputIntClamped(string label, ref int v, int step, int min, int max, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        var modified = ImGui.InputInt(label, ref v, step, step, flags);
        if (!modified) return false;

        v = MathHelper.Clamp(v, min, max);
        return true;
    }

    public static bool DragIntClamped(string label, ref int v, float v_speed, int v_min, int v_max, string? format = null, ImGuiSliderFlags flags = ImGuiSliderFlags.None)
    {
        var modified = ImGui.DragInt(label, ref v, v_speed, v_min, v_max, format, flags);
        if (!modified) return false;

        v = MathHelper.Clamp(v, v_min, v_max);
        return true;
    }

    public static bool DragFloatClamped(string label, ref float v, float v_speed, float v_min, float v_max, string? format = null, ImGuiSliderFlags flags = ImGuiSliderFlags.None)
    {
        var modified = ImGui.DragFloat(label, ref v, v_speed, v_min, v_max, format, flags);
        if (!modified) return false;

        v = MathHelper.Clamp(v, v_min, v_max);
        return true;
    }
}
