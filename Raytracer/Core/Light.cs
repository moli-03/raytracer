using Raytracer.Core.Math;

namespace Raytracer.Core;

public class Light
{
    public Transform transform { get; }
    public Color color { get; }

    public Light(Color color)
    {
        this.color = color;
        this.transform = new Transform(Vector3.Zero, Quaternion.Identity);
    }
}