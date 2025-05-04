using Raytracer.Core.Math;

namespace Raytracer.Core.Objects;

public class Mesh : BaseObject
{
    private List<Triangle> triangles = new();
    public string Name { get; private set; }

    public Mesh(string name)
    {
        this.Name = name;
    }
    
    public int GetTriangleCount() => triangles.Count;

    public void AddTriangle(Triangle triangle)
    {
        triangle.transform.parent = transform;
        triangles.Add(triangle);
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
