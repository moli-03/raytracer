namespace Raytracer.Core.Materials;

public class Ruby() : Material(
    new Color(0.1745f, 0.01175f, 0.01175f),
    new Color(0.61424f, 0.04136f, 0.04136f),
    new Color(0.727811f, 0.626959f, 0.626959f),
    0.6f,     // Reflectivity
    0.3f,     // Some transparency
    1.76f     // Actual ruby refractive index
);