namespace Raytracer.Core;

public class Level : BaseObject
{
    public Vector3 V { get; }
    public Vector3 W { get; }
    public Vector3 Normal { get; }
    
    public Level(Vector3 origin, Vector3 v, Vector3 w)
    {
        // Set the plane's position to the origin
        this.transform.MoveTo(origin);
        
        // V and W are already the directions defining the plane; they don't need to be offset by origin.
        V = v;
        W = w;
        
        // Calculate the normal of the plane as the cross product of V and W
        Normal = Vector3.Cross(V, W).Normalized;
    }

    public override bool Collides(Ray ray, out RayHit hit)
    {
        var b = this.transform.Position - ray.Origin;

        var A = new Matrix3x3(
            ray.Direction.X, -V.X, -W.X,
            ray.Direction.Y, -V.Y, -W.Y,
            ray.Direction.Z, -V.Z, -W.Z
        );

        if (!A.TryGetInverse(out Matrix3x3 invA))
        {
            hit = new RayHit() { HasHit = false };
            return false;
        }

        var result = invA * b;
        float t = result.X; // Distance along ray
        float u = result.Y; // Plane parameter u
        float v = result.Z; // Plane parameter v

        if (t < 0)
        {
            hit = new RayHit() { HasHit = false };
            return false; // Intersection is behind the ray's origin
        }

        var hitPosition = ray.Origin + t * ray.Direction;

        hit = new RayHit()
        {
            HasHit = true,
            HitObject = this,
            Distance = t,
            Position = hitPosition,
            Normal = Normal
        };
        return true;
    }
}
