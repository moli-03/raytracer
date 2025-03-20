namespace Raytracer.Core;

public class Cube : BaseObject
{
    public float SideLength { get; }
    
    private List<Triangle> triangles = new List<Triangle>();

    public Cube(float sideLength)
    {
        SideLength = sideLength;
        
        var top1 = new Triangle(
            new Vector3(-sideLength / 2, sideLength / 2, -sideLength / 2),
            new Vector3(-sideLength / 2, sideLength / 2, sideLength / 2),
            new Vector3(sideLength / 2, sideLength / 2, -sideLength / 2)
            );
        top1.transform.parent = transform;
        this.triangles.Add(top1);
        
        var top2 = new Triangle(
            new Vector3(-sideLength / 2, sideLength / 2, sideLength / 2),
            new Vector3(sideLength / 2, sideLength / 2, sideLength / 2),
            new Vector3(sideLength / 2, sideLength / 2, -sideLength / 2)
            );
        top2.transform.parent = transform;
        this.triangles.Add(top2);
        
        var right1 = new Triangle(
            new Vector3(sideLength / 2, sideLength / 2, -sideLength / 2),
            new Vector3(sideLength / 2, sideLength / 2, sideLength / 2),
            new Vector3(sideLength / 2, -sideLength / 2, -sideLength / 2)
            );
        right1.transform.parent = transform;
        this.triangles.Add(right1);
        
        var right2 = new Triangle(
            new Vector3(sideLength / 2, -sideLength / 2, -sideLength / 2),
            new Vector3(sideLength / 2, sideLength / 2, sideLength / 2),
            new Vector3(sideLength / 2, -sideLength / 2, sideLength / 2)
            );
        right2.transform.parent = transform;
        this.triangles.Add(right2);
        
        
        var back1 = new Triangle(
            new Vector3(sideLength / 2, sideLength / 2, sideLength / 2),
            new Vector3(-sideLength / 2, sideLength / 2, sideLength / 2),
            new Vector3(sideLength / 2, -sideLength / 2, sideLength / 2)
            );
        back1.transform.parent = transform;
        this.triangles.Add(back1);
        
        var back2 = new Triangle(
            new Vector3(-sideLength / 2, sideLength / 2, sideLength / 2),
            new Vector3(-sideLength / 2, -sideLength / 2, sideLength / 2),
            new Vector3(sideLength / 2, -sideLength / 2, sideLength / 2)
            );
        back2.transform.parent = transform;
        this.triangles.Add(back2);
        
        var bottom1 = new Triangle(
            new Vector3(sideLength / 2, -sideLength / 2, -sideLength / 2),
            new Vector3(sideLength / 2, -sideLength / 2, sideLength / 2),
            new Vector3(-sideLength / 2, -sideLength / 2, sideLength / 2)
            );
        bottom1.transform.parent = transform;
        this.triangles.Add(bottom1);
        
        var bottom2 = new Triangle(
            new Vector3(sideLength / 2, -sideLength / 2, -sideLength / 2),
            new Vector3(-sideLength / 2, -sideLength / 2, sideLength / 2),
            new Vector3(-sideLength / 2, -sideLength / 2, -sideLength / 2)
            );
        bottom2.transform.parent = transform;
        this.triangles.Add(bottom2);
        
        var left1 = new Triangle(
            new Vector3(-sideLength / 2, -sideLength / 2, -sideLength / 2),
            new Vector3(-sideLength / 2, -sideLength / 2, sideLength / 2),
            new Vector3(-sideLength / 2, sideLength / 2, -sideLength / 2)
            );
        left1.transform.parent = transform;
        this.triangles.Add(left1);
        
        var left2 = new Triangle(
            new Vector3(-sideLength / 2, sideLength / 2, -sideLength / 2),
            new Vector3(-sideLength / 2, -sideLength / 2, sideLength / 2),
            new Vector3(-sideLength / 2, sideLength / 2, sideLength / 2)
            );
        left2.transform.parent = transform;
        this.triangles.Add(left2);
        
        var front1 = new Triangle(
            new Vector3(-sideLength / 2, -sideLength / 2, -sideLength / 2),
            new Vector3(-sideLength / 2, sideLength / 2, -sideLength / 2),
            new Vector3(sideLength / 2, -sideLength / 2, -sideLength / 2)
            );
        front1.transform.parent = transform;
        this.triangles.Add(front1);
        
        var front2 = new Triangle(
            new Vector3(-sideLength / 2, sideLength / 2, -sideLength / 2),
            new Vector3(sideLength / 2, sideLength / 2, -sideLength / 2),
            new Vector3(sideLength / 2, -sideLength / 2, -sideLength / 2)
            );
        front2.transform.parent = transform;
        this.triangles.Add(front2);
    }

    public override bool Collides(Ray ray, out RayHit hit)
    {
        
        RayHit? closestHit = null;
        foreach (var triangle in triangles)
        {
            if (!triangle.Collides(ray, out RayHit planeHit))
            {
                continue;
            }
            
            if (!closestHit.HasValue || planeHit.Distance < closestHit?.Distance)
            {
                closestHit = planeHit;
            }
        }

        if (closestHit == null)
        {
            hit = new RayHit() { HasHit = false };
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