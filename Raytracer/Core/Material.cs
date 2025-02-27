
using System.Drawing;

namespace Raytracer.Core {

	public class Material {

		public RaytracingColor color;

		private Material(RaytracingColor color) {
			this.color = color;
		}

		public static Material SingleColor(RaytracingColor color) {
			return new Material(color);
		}

	}

}