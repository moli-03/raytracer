
namespace Raytracer.Core {
	public class Scene
	{
		public Camera camera = new Camera(Vector3.Zero, new Vector3(0, 0, 1), new Vector3(0, 1, 0), 100f);
    	public List<BaseObject> Objects { get; } = new List<BaseObject>();

		private RayHit noHit = new RayHit {
			HasHit = false,
			Position = null
		};

    	public void AddObject(BaseObject sphere)
    	{
        	Objects.Add(sphere);
    	}

    	public RayHit TraceRay(Ray ray)
	    {
		    RayHit? closestHit = null;
			RaytracingColor? resultColor = null;
			
        	foreach (var obj in Objects)
        	{
            	if (obj.Collides(ray, out RayHit hit))
            	{
					if (hit.HasHit) {
						Console.WriteLine("I have hit something. Color: " + hit.Color.ToString());
						if (!closestHit.HasValue) {
							closestHit = hit;
							resultColor = hit.Color;
							continue;
						}

						if (hit.Distance < closestHit.Value.Distance)
						{
							closestHit = hit;
						}

						resultColor += hit.Color;
					}
            	}
        	}

	        RayHit? result = null;
	        if (closestHit.HasValue)
	        {
		        result = new RayHit()
		        {
			        HasHit = true,
			        Distance = closestHit.Value.Distance,
			        Position = closestHit.Value.Position,
			        HitObject = closestHit.Value.HitObject,
			        Color = resultColor!.Value
		        };
	        }
			return result ?? new RayHit { HasHit = false };
		}
	}

}