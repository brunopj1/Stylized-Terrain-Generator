﻿namespace Engine.Extensions;
public static class VectorExtensions
{
    public static System.Numerics.Vector2 ToNumerics(this Vector2 vector)
    {
        return new(vector.X, vector.Y);
    }

    public static System.Numerics.Vector3 ToNumerics(this Vector3 vector)
    {
        return new(vector.X, vector.Y, vector.Z);
    }

    public static System.Numerics.Vector4 ToNumerics(this Vector4 vector)
    {
        return new(vector.X, vector.Y, vector.Z, vector.W);
    }

    public static Vector2 ToOpenTK(this System.Numerics.Vector2 vector)
    {
        return new(vector.X, vector.Y);
    }

    public static Vector3 ToOpenTK(this System.Numerics.Vector3 vector)
    {
        return new(vector.X, vector.Y, vector.Z);
    }

    public static Vector4 ToOpenTK(this System.Numerics.Vector4 vector)
    {
        return new(vector.X, vector.Y, vector.Z, vector.W);
    }

    public static uint ToHexColor(this Vector3 vector)
    {
        return (uint)(vector.X * 255) << 16 | (uint)(vector.Y * 255) << 8 | (uint)(vector.Z * 255);
    }

    public static uint ToHexColor(this Vector4 vector)
    {
        return (uint)(vector.X * 255) << 24 | (uint)(vector.Y * 255) << 16 | (uint)(vector.Z * 255) << 8 | (uint)(vector.W * 255);
    }
}
