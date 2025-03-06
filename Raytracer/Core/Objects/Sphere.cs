using System;
using System.Numerics; // Ensure using System.Numerics for Vector3

namespace Raytracer.Core
{
    public class Sphere : BaseObject
    {
        public float Radius { get; }

        public Sphere(float radius)
        {
            this.Radius = radius;
        }

        public override bool Collides(Ray ray, out RayHit hit)
        {
            Vector3 oc = ray.Origin - this.transform.Position;
            float a = Vector3.Dot(ray.Direction, ray.Direction);
            float b = 2.0f * Vector3.Dot(oc, ray.Direction);
            float c = Vector3.Dot(oc, oc) - Radius * Radius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                hit = new RayHit { HasHit = false };
                return false;
            }

            // Compute the two possible intersection points
            float sqrtDiscriminant = (float)Math.Sqrt(discriminant);
            float t1 = (-b - sqrtDiscriminant) / (2.0f * a);
            float t2 = (-b + sqrtDiscriminant) / (2.0f * a);

            // Choose the closest valid t
            float distance = (t1 > 0) ? t1 : ((t2 > 0) ? t2 : -1);
            if (distance < 0) 
            {
                hit = new RayHit { HasHit = false };
                return false;
            }

            // Correct normal calculation
            Vector3 hitPosition = ray.Origin + ray.Direction * distance;
            Vector3 normal = (hitPosition - transform.Position).Normalized;

            hit = new RayHit
            {
                HasHit = true,
                Position = hitPosition,
                Distance = distance,
                HitObject = this,
                Normal = normal
            };
            return true;
        }
    }
}
