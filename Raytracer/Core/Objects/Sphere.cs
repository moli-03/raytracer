using System.Drawing;

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
            Vector3 oc = ray.Origin - Transform.Position;
            float a = Vector3.Dot(ray.Direction, ray.Direction);
            float b = 2.0f * Vector3.Dot(oc, ray.Direction);
            float c = Vector3.Dot(oc, oc) - Radius * Radius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant > 0)
            {
                float distance = (-b - (float)Math.Sqrt(discriminant)) / (2.0f * a);
                hit = new RayHit {
                    HasHit = true,
                    Position = ray.Origin + ray.Direction * distance,
                    Distance = distance,
                    HitObject = this
                };
                return true;
            }

            hit = new RayHit { HasHit = false };
            return false;
        }
    }
}