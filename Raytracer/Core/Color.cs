namespace Raytracer.Core;

public struct Color
{
    public float R, G, B;

    public Color(float r, float g, float b)
    {
        R = System.Math.Clamp(r, 0, 1);
        G = System.Math.Clamp(g, 0, 1);
        B = System.Math.Clamp(b, 0, 1);
    }
    
    
    public static Color operator +(Color a, Color b)
    {
        return new Color(a.R + b.R, a.G + b.G, a.B + b.B);
    }
    
    public static Color operator *(Color a, Color b)
    {
        return new Color(a.R * b.R, a.G * b.G, a.B * b.B);
    }
    
    public static Color operator +(float a, Color b)
    {
        return new Color(a + b.R, a + b.G, a + b.B);
    }
    
    public static Color operator +(Color a, float b)
    {
        return new Color(a.R + b, a.G + b, a.B + b);
    }
    
    public static Color operator *(float a, Color b)
    {
        return new Color(a * b.R, a * b.G, a * b.B);
    }
    
    public static Color operator *(Color a, float b)
    {
        return new Color(a.R * b, a.G * b, a.B * b);
    }

    public System.Drawing.Color ToSystemColor()
    {
        return System.Drawing.Color.FromArgb((int)(R * 255), (int)(G * 255), (int)(B * 255));
    }

    public override string ToString()
    {
        return $"R: {R}, G: {G}, B: {B}";
    }
}