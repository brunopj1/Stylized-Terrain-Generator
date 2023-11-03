using Engine.Extensions;
using OpenTK.Mathematics;

namespace Engine.Graphics;

public abstract class BoundingVolume
{
    internal abstract bool IsOnFrustum(Matrix4 modelMatrix, IEnumerable<Vector4> frustumPlanes);
}

public class BoundingSphere : BoundingVolume
{
    public BoundingSphere(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public Vector3 Center { get; set; }
    public float Radius { get; set; }

    internal override bool IsOnFrustum(Matrix4 modelMatrix, IEnumerable<Vector4> frustumPlanes)
    {
        var tempCenter = Center;
        var tempRadius = Radius;

        if (modelMatrix != Matrix4.Identity)
        {
            var scale = modelMatrix.ExtractScale();
            tempCenter = (new Vector4(Center, 1f) * modelMatrix).Xyz;
            tempRadius = MathF.Max(MathF.Max(MathF.Abs(scale.X), MathF.Abs(scale.Y)), MathF.Abs(scale.Z));
        }

        foreach (var plane in frustumPlanes)
        {
            if (ViewFrustumHelper.GetSignedDistance(plane, tempCenter) < -tempRadius) return false;
        }
        return true;
    }
}

public class AxisAlignedBoundingBox : BoundingVolume
{
    public AxisAlignedBoundingBox(Vector3 min, Vector3 max)
    {
        Center = (min + max) * 0.5f;
        Extents = max - Center;
    }

    public AxisAlignedBoundingBox(Vector3 center, float extentX, float extentY, float extentZ)
    {
        Center = center;
        Extents = new(extentX, extentY, extentZ);
    }

    public Vector3 Center { get; set; }
    public Vector3 Extents { get; set; }

    internal override bool IsOnFrustum(Matrix4 modelMatrix, IEnumerable<Vector4> frustumPlanes)
    {
        var tempCenter = Center;
        var tempExtends = Extents;

        if (modelMatrix != Matrix4.Identity)
        {
            tempCenter = (new Vector4(tempCenter, 1f) * modelMatrix).Xyz;

            var right = modelMatrix.Column0.Xyz * tempExtends.X;
            var up = modelMatrix.Column1.Xyz * tempExtends.Y;
            var forward = modelMatrix.Column2.Xyz * tempExtends.Z;

            Vector3 x = Vector3.UnitX, y = Vector3.UnitY, z = Vector3.UnitZ;

            var newIi = MathF.Abs(Vector3.Dot(x, right)) + MathF.Abs(Vector3.Dot(x, up)) + MathF.Abs(Vector3.Dot(x, forward));
            var newIj = MathF.Abs(Vector3.Dot(y, right)) + MathF.Abs(Vector3.Dot(y, up)) + MathF.Abs(Vector3.Dot(y, forward));
            var newIk = MathF.Abs(Vector3.Dot(z, right)) + MathF.Abs(Vector3.Dot(z, up)) + MathF.Abs(Vector3.Dot(z, forward));

            tempExtends = new Vector3(newIi, newIj, newIk);
        }

        foreach (var plane in frustumPlanes)
        {
            var r = tempExtends.X * MathF.Abs(plane.X) + tempExtends.Y * MathF.Abs(plane.Y) + tempExtends.Z * MathF.Abs(plane.Z);
            if (ViewFrustumHelper.GetSignedDistance(plane, tempCenter) < -r) return false;
        }
        return true;
    }
}

public class MultiBoundingVolume : BoundingVolume
{
    public MultiBoundingVolume(IEnumerable<BoundingVolume> boundingVolumes)
    {
        BoundingVolumes = boundingVolumes;
    }

    public IEnumerable<BoundingVolume> BoundingVolumes { get; set; }

    internal override bool IsOnFrustum(Matrix4 modelMatrix, IEnumerable<Vector4> frustumPlanes)
    {
        foreach (var boundingVolume in BoundingVolumes)
        {
            if (boundingVolume.IsOnFrustum(modelMatrix, frustumPlanes)) return true;
        }
        return false;
    }
}
