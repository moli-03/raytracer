using Raytracer.Core.Math;

namespace Raytracer.Core.Objects;

public class Plane : BaseObject
{
    
    public float Width { get; }
    public float Length { get; }
    
    private List<Triangle> triangles = new();

    public Plane(float width, float length)
    {
        Width = width;
        Length = length;
        
        triangles.Add(new Triangle(
            new Vector3(-width / 2, 0, -length / 2),
            new Vector3(-width / 2, 0, length / 2),
            new Vector3(width / 2, 0, -length / 2)
            ));
        
        triangles.Add(new Triangle(
            new Vector3(-width / 2, 0, length / 2),
            new Vector3(width / 2, 0, length / 2),
            new Vector3(width / 2, 0, -length / 2)
            ));

        foreach (var triangle in triangles)
        {
            triangle.transform.parent = transform;
        }
    }

    public override bool Collides(Ray ray, out RayHit hit)
    {
        RayHit? closestHit = null;
        foreach (var triangle in triangles)
        {
            if (!triangle.Collides(ray, out RayHit triangleHit))
            {
                continue;
            }
            
            if (!closestHit.HasValue || triangleHit.Distance < closestHit?.Distance)
            {
                closestHit = triangleHit;
            }
        }

        if (closestHit == null)
        {
            hit = RayHit.NoHit;
            return false;
        }

        hit = new RayHit()
        {
            HasHit = true,
            Position = closestHit.Value.Position,
            Distance = closestHit.Value.Distance,
            HitObject = this,
            Normal = closestHit.Value.Normal
        };
        
        return true;
    }
}
