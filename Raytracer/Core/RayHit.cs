using Raytracer.Core.Math;
using Raytracer.Core.Objects;

namespace Raytracer.Core;

public struct RayHit {
	
	public static readonly RayHit NoHit = new RayHit() { HasHit = false};

	public bool HasHit;
	public Vector3? Position;
	public float Distance;
	public BaseObject HitObject;
	public Vector3? Normal;
}
