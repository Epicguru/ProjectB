
using System.Collections.Generic;
using UnityEngine;

namespace ProjectB.Physics
{
    public static class RigidbodyExtensions
    {
        private static List<Collider2D> tempColliders = new List<Collider2D>();

        public static Bounds[] GetAllBounds(this Rigidbody2D body)
        {
            int count = body.GetAttachedColliders(tempColliders);
            Bounds[] bounds = new Bounds[count];
            for (int i = 0; i < count; i++)
            {
                bounds[i] = tempColliders[i].bounds;
            }

            return bounds;
        }

        public static Bounds GetBounds(this Rigidbody2D body)
        {
            var bounds = body.GetAllBounds();
            Vector3 rMin = new Vector3(float.MaxValue, float.MaxValue, 0f);
            Vector3 rMax = new Vector3(float.MinValue, float.MinValue, 0f);
            foreach (var bound in bounds)
            {
                Vector3 min = bound.min;
                Vector3 max = bound.max;

                if (min.x < rMin.x)
                    rMin.x = min.x;
                if (min.y < rMin.y)
                    rMin.y = min.y;
                if (max.x > rMax.x)
                    rMax.x = max.x;
                if (max.y > rMax.y)
                    rMax.y = max.y;
            }

            Bounds final = new Bounds();
            final.SetMinMax(rMin, rMax);

            return final;
        }

        public static string MovementStats(this Rigidbody2D body)
        {
            return $"Mass: {body.mass:F1} kg, Velocity: {body.velocity.magnitude:F1} ㎧, A.Vel: {(body.angularVelocity / 360f * 60f):F1} rpm";
        }

        public static Vector2 GetAproxAcceletation(this Rigidbody2D body, Vector2 lastVelocity, float deltaTime)
        {
            Vector2 diff = body.velocity - lastVelocity;
            return diff / deltaTime;
        }
    }
}
