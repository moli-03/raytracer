namespace Raytracer.Core;

public class Cube : BaseObject
{
    public float SideLength { get; }
    
    private List<Plane> planes = new List<Plane>();

    public Cube(float sideLength)
    {
        SideLength = sideLength;
        
        var top = new Plane(sideLength, sideLength);
        top.transform.MoveTo(0, sideLength / 2, 0);
        this.planes.Add(top);
        
        var bottom = new Plane(sideLength, sideLength);
        bottom.transform.MoveTo(0, -sideLength / 2, 0);
        bottom.transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, (float)Math.PI);
        this.planes.Add(bottom);
        
        var left = new Plane(sideLength, sideLength);
        left.transform.MoveTo(-sideLength / 2, 0, 0);
        left.transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, (float)Math.PI / 2);
        this.planes.Add(left);
        
        var right = new Plane(sideLength, sideLength);
        right.transform.MoveTo(sideLength / 2, 0, 0);
        right.transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, -(float)Math.PI / 2);
        this.planes.Add(right);
        
        var back = new Plane(sideLength, sideLength);
        back.transform.MoveTo(0, 0, sideLength / 2);
        back.transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, (float)Math.PI / 2);
        this.planes.Add(back);
        
        var front = new Plane(sideLength, sideLength);
        front.transform.MoveTo(0, 0, -sideLength / 2);
        front.transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, -(float)Math.PI / 2);
        this.planes.Add(front);
        
        

        foreach (var plane in planes)
        {
            plane.transform.Parent = transform;
        }
    }

    public override bool Collides(Ray ray, out RayHit hit)
    {
        
        RayHit? closestHit = null;
        foreach (var plane in planes)
        {
            if (!plane.Collides(ray, out RayHit planeHit))
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