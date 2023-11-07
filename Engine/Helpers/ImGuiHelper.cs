using ImGuiNET;

namespace Engine.Helpers;
public static class ImGuiHelper
{
    public static bool DragFloat(string label, ref float v, float v_speed, float v_min, float v_max)
    {
        bool modified = ImGui.DragFloat(label, ref v, v_speed, v_min, v_max);
        if (!modified) return false;

        v = MathHelper.Clamp(v, v_min, v_max);
        return true;
    }

    public static bool DragInt(string label, ref int v, float v_speed, int v_min, int v_max)
    {
        bool modified = ImGui.DragInt(label, ref v, v_speed, v_min, v_max);
        if (!modified) return false;

        v = MathHelper.Clamp(v, v_min, v_max);
        return true;
    }
}
