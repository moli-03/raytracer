namespace Raytracer.Core.Materials;

public class Glass() : Material(
    new Color(0.02f, 0.05f, 0.05f),    // Very low ambient
    new Color(0.2f, 0.2f, 0.2f),       // Low diffuse
    new Color(1.0f, 1.0f, 1.0f),       // High specular
    0.1f,     // Lower reflectivity
    0.95f,    // Higher transparency
    1.5f     // Glass refractive index
);
