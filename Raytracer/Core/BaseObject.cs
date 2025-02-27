using System.Drawing;

namespace Raytracer.Core {

	public abstract class BaseObject
	{

		public Transform transfrom;
		public Material material;

		public BaseObject(Vector3 position, Quaternion rotation) {
			this.transfrom = new Transform(position, rotation);
			this.material = Raytracer.Core.Material.SingleColor(new RaytracingColor(0, 0, 1));
		}

		public BaseObject(Vector3 position) : this (position, Quaternion.Identity) { }

		public BaseObject(Quaternion rotation): this (Vector3.Zero, rotation) { }

		public BaseObject(): this(Vector3.Zero, Quaternion.Identity) { }

		public abstract bool Collides(Ray ray, out RayHit hit);
	}

}