using System.Drawing;

namespace Raytracer.Core {

	public abstract class BaseObject {

		public Transform Transform { get; }
		public Material Material { get; set; }

		public BaseObject(Vector3 position, Quaternion rotation) {
			this.Transform = new Transform(position, rotation);
			this.Material = Raytracer.Core.Material.SingleColor(Color.Cyan);
		}

		public BaseObject(Vector3 position) : this (position, Quaternion.Identity) { }

		public BaseObject(Quaternion rotation): this (Vector3.Zero, rotation) { }

		public BaseObject(): this(Vector3.Zero, Quaternion.Identity) { }

		public abstract bool Collides(Ray ray, out RayHit hit);
	}

}