using Raytracer.Core.Math;

namespace Raytracer.Core;

public class Camera
{
	public Vector3 Position { get; set; }
	public Vector3 LookDirection { get; set; }
	public Vector3 Up { get; set; }
	public float Fov { get; set; }

	public Camera(Vector3 position, Vector3 lookDirection, Vector3 up, float fov)
	{
		Position = position;
		LookDirection = lookDirection;
		Up = up;
		Fov = fov;
	}

	public Ray GetRay(float x, float y, int width, int height)
	{
		// Adjust for screen resolution and field of view
		float aspectRatio = (float)width / height;
		float scale = (float)System.Math.Tan(Fov * 0.5 * System.Math.PI / 180);
		float imageX = (2 * ((x + 0.5f) / width) - 1) * scale * aspectRatio;
		float imageY = (1 - 2 * ((y + 0.5f) / height)) * scale;

		// Calculate camera basis vectors
		Vector3 forward = LookDirection.Normalized;
		Vector3 right = Vector3.Cross(forward, Up).Normalized;
		Vector3 up = Vector3.Cross(right, forward);

		// Calculate ray direction using the camera basis - note the negative imageX to flip horizontally
		Vector3 rayDir = forward + right * (-imageX) + up * imageY;

		return new Ray(Position, rayDir.Normalized);
	}
}