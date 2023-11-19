using Engine.Extensions;

namespace Engine.Graphics;

public abstract class BoundingVolume
{
    internal bool WasUpdated { get; set; } = true;
    internal bool IsVisible { get; set; } = true;

    internal void UpdateVisibility(Matrix4 modelMatrix, IEnumerable<Vector4> frustumPlanes)
    {
        IsVisible = IsOnFrustum(modelMatrix, frustumPlanes);
        WasUpdated = false;
    }

    internal abstract bool IsOnFrustum(Matrix4 modelMatrix, IEnumerable<Vector4> frustumPlanes);
}

public class BoundingSphere : BoundingVolume
{
    public BoundingSphere(Vector3 center, float radius)
    {
        _center = center;
        _radius = radius;
    }

    private Vector3 _center;
    public Vector3 Center
    {
        get => _center;
        set
        {
            _center = value;
            WasUpdated = true;
        }
    }

    private float _radius;
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            WasUpdated = true;
        }
    }

    internal override bool IsOnFrustum(Matrix4 modelMatrix, IEnumerable<Vector4> frustumPlanes)
    {
        var tempCenter = _center;
        var tempRadius = _radius;

        if (modelMatrix != Matrix4.Identity)
        {
            var scale = modelMatrix.ExtractScale();
            tempCenter = (new Vector4(_center, 1f) * modelMatrix).Xyz;
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

    private Vector3 _center;
    public Vector3 Center
    {
        get => _center;
        set
        {
            _center = value;
            WasUpdated = true;
        }
    }

    private Vector3 _extents;
    public Vector3 Extents
    {
        get => _extents;
        set
        {
            _extents = value;
            WasUpdated = true;
        }
    }

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
        _boundingVolumes = boundingVolumes;
    }

    private IEnumerable<BoundingVolume> _boundingVolumes;

    public void AddBoundingVolume(BoundingVolume boundingVolume)
    {
        _boundingVolumes = _boundingVolumes.Append(boundingVolume);
        WasUpdated = true;
    }

    public void RemoveBoundingVolume(BoundingVolume boundingVolume)
    {
        _boundingVolumes = _boundingVolumes.Where(bv => bv != boundingVolume);
        WasUpdated = true;
    }

    internal override bool IsOnFrustum(Matrix4 modelMatrix, IEnumerable<Vector4> frustumPlanes)
    {
        foreach (var boundingVolume in _boundingVolumes)
        {
            if (boundingVolume.IsOnFrustum(modelMatrix, frustumPlanes)) return true;
        }
        return false;
    }
}
