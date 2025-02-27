namespace Raytracer.Core {

	public struct RayHit {

		public bool HasHit;
		public Vector3? Position;
		public float Distance;
		public BaseObject HitObject;
		public RaytracingColor Color;
	}

}