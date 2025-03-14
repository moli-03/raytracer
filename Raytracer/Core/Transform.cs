namespace Raytracer.Core {

	public class Transform
	{
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
		public Transform? Parent { get; set; } = null;

		public Transform(Vector3 position, Quaternion rotation)
		{
			this.Position = position;
			this.Rotation = rotation;
		}

		public void Translate(float x, float y, float z)
		{
			this.Position = this.Position + new Vector3(x, y, z);
		}

		public void MoveTo(float x, float y, float z)
		{
			this.Position = new Vector3(x, y, z);
		}

		public void MoveTo(Vector3 position)
		{
			this.Position = position;
		}

		public Vector3 TransformToWorldPosition(Vector3 localPosition)
		{
			// Apply the local transform first (rotation first, then translation)
			Vector3 worldPosition = Rotation * localPosition + Position;

			// If there's a parent, apply its transformation recursively
			if (Parent != null)
			{
				worldPosition = Parent.TransformToWorldPosition(worldPosition);
			}

			return worldPosition;
		}	}

}