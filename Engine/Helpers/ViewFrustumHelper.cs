namespace Engine.Extensions;

public static class ViewFrustumHelper
{
    public static Vector4 Plane(Vector3 position, Vector3 normal)
    {
        return new(normal, Vector3.Dot(normal, position));
    }

    public static float GetSignedDistance(Vector4 plane, Vector3 point)
    {
        return Vector3.Dot(plane.Xyz, point) - plane.W;
    }
}
