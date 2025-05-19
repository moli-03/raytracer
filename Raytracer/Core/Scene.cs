using Raytracer.Core.Objects;
using Raytracer.Core.Math;

namespace Raytracer.Core;

public class Scene
{
	public Camera camera = new Camera(Vector3.Zero, new Vector3(0, 0, 1), new Vector3(0, 1, 0), 75f);
	public List<BaseObject> Objects { get; } = new List<BaseObject>();
	public List<Light> Lights { get; } = new List<Light>();

	public void AddObject(BaseObject obj)
	{
		Objects.Add(obj);
	}

	public void AddLight(Light light)
	{
		Lights.Add(light);
	}
	
	public void Clear()
	{
		Objects.Clear();
		Lights.Clear();
		// Reset camera to default
		camera = new Camera(Vector3.Zero, new Vector3(0, 0, 1), new Vector3(0, 1, 0), 75f);
	}

	public RayHit TraceRay(Ray ray)
	{
		RayHit? closestHit = null;
		
		foreach (var obj in Objects)
		{
			if (obj.Collides(ray, out RayHit hit))
			{
				if (hit.HasHit) {
					if (!closestHit.HasValue) {
						closestHit = hit;
						continue;
					}

					if (hit.Distance < closestHit.Value.Distance)
					{
						closestHit = hit;
					}
				}
			}
		}

		return closestHit ?? new RayHit { HasHit = false };
	}
}