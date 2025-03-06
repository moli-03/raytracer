namespace Raytracer.Core;

public class Light
{
    public Transform transform { get; }
    public RaytracingColor color { get; }

    public Light(RaytracingColor color)
    {
        this.color = color;
        this.transform = new Transform(Vector3.Zero, Quaternion.Identity);
    }
}