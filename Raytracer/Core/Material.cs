
using System.Drawing;

namespace Raytracer.Core {

	public class Material {

		public Color color;

		private Material(Color color) {
			this.color = color;
		}

		public static Material SingleColor(Color color) {
			return new Material(color);
		}

	}

}