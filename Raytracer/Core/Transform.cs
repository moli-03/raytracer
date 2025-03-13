namespace Raytracer.Core {

	public class Transform {

		public Vector3 Position { get; private set; }
		public Quaternion Rotation { get; private set; }

		public Transform(Vector3 position, Quaternion rotation) {
			this.Position = position;
			this.Rotation = rotation;
		}

		public void Translate(float x, float y, float z) {
			this.Position = this.Position + new Vector3(x, y, z);
		}

		public void MoveTo(float x, float y, float z) {
			this.Position = new Vector3(x, y, z);
		}

		public void MoveTo(Vector3 position)
		{
			this.Position = position;
		}
	}

}