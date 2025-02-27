
using System.Drawing;

namespace Raytracer.Core;

public struct RaytracingColor
{
    public float R, G, B;

    public RaytracingColor(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }
    
    
    public static RaytracingColor operator +(RaytracingColor a, RaytracingColor b)
    {
        return new RaytracingColor(a.R + b.R, a.G + b.G, a.B + b.B);
    }
    
    public static RaytracingColor operator *(RaytracingColor a, RaytracingColor b)
    {
        return new RaytracingColor(a.R * b.R, a.G * b.G, a.B * b.B);
    }

    public Color ToColor()
    {
        return Color.FromArgb((int)(R * 255), (int)(G * 255), (int)(B * 255));
    }

    public override string ToString()
    {
        return $"R: {R}, G: {G}, B: {B}";
    }
}