namespace Raytracer.Core.Objects.Materials;

public class Material {

	public Color Ambient;
	public Color Diffuse;
	public Color Specular;
	public float Reflectivity = 0f;

	public Material(Color ambient, Color diffuse, Color specular, float reflectivity = 0f)
	{
		this.Ambient = ambient;
		this.Diffuse = diffuse;
		this.Specular = specular;
		this.Reflectivity = reflectivity;
	}

	public static Material SingleColor(Color color) {
		return new Material(color, color, color);
	}

}