namespace Raytracer.Core;

public class Triangle : BaseObject
{
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    private Vector3 worldV => TransformPoint(p2) - TransformPoint(p1);
    private Vector3 worldW => TransformPoint(p3) - TransformPoint(p1);

    public Vector3 Normal => Vector3.Cross(worldV, worldW).Normalized;

    public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }

    public override bool Collides(Ray ray, out RayHit hit)
    {
        var worldP1 = TransformPoint(p1);
        var b = worldP1 - ray.Origin;

        var A = new Matrix3x3(
            ray.Direction.X, -worldV.X, -worldW.X,
            ray.Direction.Y, -worldV.Y, -worldW.Y,
            ray.Direction.Z, -worldV.Z, -worldW.Z
        );

        if (!A.TryGetInverse(out Matrix3x3 invA))
        {
            hit = RayHit.NoHit;
            return false;
        }

        var result = invA * b;
        float lambda = result.X;
        float mu = result.Y;
        float tau = result.Z;

        if (lambda < 0 || mu < 0 || tau < 0 || mu + tau > 1)
        {
            hit = RayHit.NoHit;
            return false;
        }

        var hitPosition = ray.Origin + lambda * ray.Direction;

        hit = new RayHit
        {
            HasHit = true,
            HitObject = this,
            Distance = lambda,
            Position = hitPosition,
            Normal = Normal // Use world normal if needed
        };
        return true;
    }

    private Vector3 TransformPoint(Vector3 localPoint)
    {
        return this.transform.TransformToWorldPosition(localPoint); // Assuming transform exists
    }
}
