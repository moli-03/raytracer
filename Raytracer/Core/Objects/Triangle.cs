namespace Raytracer.Core.Objects;

public class Triangle : BaseObject
{
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    private Vector3 worldP1;
    private Vector3 worldP2;
    private Vector3 worldP3;
    private Vector3 edge1;  // P2 - P1
    private Vector3 edge2;  // P3 - P1
    private Vector3 normal;

    public Vector3 Normal => normal;

    public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
        
        this.Recalculate();
        this.transform.PositionChanged += Recalculate;
        this.transform.RotationChanged += Recalculate;
    }

    private void Recalculate()
    {
        // Transform vertices to world space
        worldP1 = TransformPoint(p1);
        worldP2 = TransformPoint(p2);
        worldP3 = TransformPoint(p3);

        // Precompute edges
        edge1 = worldP2 - worldP1;
        edge2 = worldP3 - worldP1;

        // Calculate normal
        normal = Vector3.Cross(edge1, edge2).Normalized;
    }

    public override bool Collides(Ray ray, out RayHit hit)
    {
        // Implement Möller–Trumbore ray-triangle intersection algorithm
        // which is much faster than the matrix inverse method

        Vector3 h = Vector3.Cross(ray.Direction, edge2);
        float a = Vector3.Dot(edge1, h);

        // If a is too close to 0, ray is parallel to triangle
        if (a > -1e-6f && a < 1e-6f)
        {
            hit = RayHit.NoHit;
            return false;
        }

        float f = 1.0f / a;
        Vector3 s = ray.Origin - worldP1;
        float u = f * Vector3.Dot(s, h);

        // If u is not within [0,1], intersection point is outside triangle
        if (u < 0.0f || u > 1.0f)
        {
            hit = RayHit.NoHit;
            return false;
        }

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(ray.Direction, q);

        // If v is not within [0,1] or u+v > 1, intersection point is outside triangle
        if (v < 0.0f || u + v > 1.0f)
        {
            hit = RayHit.NoHit;
            return false;
        }

        // At this point we have a valid intersection
        float t = f * Vector3.Dot(edge2, q);

        // Verify intersection is in front of ray
        if (t < 1e-6f)
        {
            hit = RayHit.NoHit;
            return false;
        }

        Vector3 hitPosition = ray.Origin + t * ray.Direction;

        hit = new RayHit
        {
            HasHit = true,
            HitObject = this,
            Distance = t,
            Position = hitPosition,
            Normal = normal
        };
        return true;
    }

    private Vector3 TransformPoint(Vector3 localPoint)
    {
        return this.transform.TransformToWorldPosition(localPoint);
    }
}