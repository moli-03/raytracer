
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

}