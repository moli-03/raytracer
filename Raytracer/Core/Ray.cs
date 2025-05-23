using Raytracer.Core.Math;

namespace Raytracer.Core;

public struct Ray
{
	public Vector3 Origin { get; }
	public Vector3 Direction { get; }

	public Ray(Vector3 origin, Vector3 direction)
	{
		Origin = origin;
		Direction = direction;
	}
}