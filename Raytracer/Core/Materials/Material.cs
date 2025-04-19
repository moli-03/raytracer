namespace Raytracer.Core.Materials;

public class Material {

	public Color Ambient;
	public Color Diffuse;
	public Color Specular;
	public float Reflectivity = 0f;
	public float RefractiveIndex = 1f;
	public float Transparency = 0f;

	public Material(Color ambient, Color diffuse, Color specular, float reflectivity = 0f, float transparency = 0f, float refractiveIndex = 1f)
	{
		this.Ambient = ambient;
		this.Diffuse = diffuse;
		this.Specular = specular;
		this.Reflectivity = reflectivity;
		this.Transparency = transparency;
		this.RefractiveIndex = refractiveIndex;
	}

	public static Material SingleColor(Color color) {
		return new Material(color, color, color);
	}

}