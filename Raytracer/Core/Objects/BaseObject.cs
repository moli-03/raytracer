using Raytracer.Core.Materials;
using Raytracer.Core.Math;

namespace Raytracer.Core.Objects;

public abstract class BaseObject
{

	public Transform transform;
	public Material material;

	public BaseObject(Vector3 position, Quaternion rotation) {
		this.transform = new Transform(position, rotation);
		this.material = MaterialLibrary.WhitePlastic;
	}

	public BaseObject(Vector3 position) : this (position, Quaternion.Identity) { }

	public BaseObject(Quaternion rotation): this (Vector3.Zero, rotation) { }

	public BaseObject(): this(Vector3.Zero, Quaternion.Identity) { }

	public abstract bool Collides(Ray ray, out RayHit hit);
}
