﻿using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Gets the angle, in degrees, from this vector towards the target vector. Assumes both this vector and the target
    /// vector are points.
    /// </summary>
    /// <param name="start">The starting point.</param>
    /// <param name="target">The target point.</param>
    /// <returns>The angle, in degrees, from this point towards the target point.</returns>
    public static float AngleTowards(this Vector2 start, Vector2 target)
    {
        return (target - start).ToAngle();
    }

    /// <summary>
    /// Get the angle, in degrees, that this vector represents as a direction.
    /// </summary>
    /// <returns>The angle in degress that this vector represents as direction around (0, 0).</returns>
    public static float ToAngle(this Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Gets the linear distance between this vector and the target vector.
    /// </summary>
    public static float DistanceTo(this Vector2 vector, Vector2 target)
    {
        return (target - vector).magnitude;
    }

    /// <summary>
    /// Compare the distance between this point and the target point, and return true if the distance is smaller or equal to the maxDistance argument.
    /// This is faster than calculating the distance because it avoids calculating square roots.
    /// </summary>
    public static bool DistanceCheck(this Vector2 vector, Vector2 target, float maxDistance)
    {
        float sqrDst = (target - vector).sqrMagnitude;
        float sqrMax = maxDistance * maxDistance;

        return sqrDst <= sqrMax;
    }

    /// <summary>
    /// Gets a vector representation of the direciton represented by this angle. The angle must be in degrees. The resulting direction vector is normalized.
    /// </summary>
    public static Vector2 ToDirection(this float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public static bool Intersects(this RectInt rect, RectInt other)
    {
        // If the max is greater than the min.
        if (rect.max.x > other.min.x && rect.max.y > other.min.y && rect.max.x <= other.max.x && rect.max.y <= other.max.y)
            return true;

        if (rect.min.x < other.max.x && rect.min.y < other.max.y && rect.min.x >= other.min.x && rect.min.y >= other.min.y)
            return true;

        return false;
    }

    public static RectInt Rotated(this RectInt rect)
    {
        return new RectInt(rect.position, new Vector2Int(rect.height, rect.width));
    }

    public static void ClampTo(this ref Rect self, Rect container)
    {
        self.x = Mathf.Max(container.x, self.x);
        self.y = Mathf.Max(container.y, self.y);

        self.x = Mathf.Min(container.x + container.width, self.x + self.width) - self.width;
        self.y = Mathf.Min(container.y + container.height, self.y + self.height) - self.height;
    }
}
