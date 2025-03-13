using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Raytracer.Core;

namespace Raytracer {

	public class RayTracingApplication {

		private WriteableBitmap frame;
		private Image screen;
		private Scene scene;

		private float diffuseColor = 0.1f;

		public RayTracingApplication(Image screen)
		{
			this.scene = new Scene();
			this.screen = screen;
            this.frame = new WriteableBitmap(500, 500, 96, 96, PixelFormats.Bgr32, null);
			this.screen.Source = this.frame;
		}

		public void Run() {

			var origin = new Vector3(0, 0, 7);
			float length = 8f;
			float width = 10f;
			float height = 10f;
			
			Light light = new Light(new RaytracingColor(1, 1, 1));
			light.transform.MoveTo(0, 0, length / 2);
			this.scene.AddLight(light);
			
			var random = new Random();
			
			// Some spheres
			for (int i = 0; i < 30; i++)
			{
				Sphere sphere = new Sphere(1.7f * random.NextSingle());
				sphere.material.color = new RaytracingColor(0, Math.Min(random.NextSingle(), 0.7f), Math.Min(random.NextSingle(), 0.6f));
				float x = random.NextSingle() * width - width / 2;
				float y = random.NextSingle() * height - height / 2;
				float z = Math.Clamp(random.NextSingle() * length - length / 2, 1f, length);
				sphere.transform.MoveTo(origin.X + x, origin.Y + y, origin.Z + z);
				this.scene.AddObject(sphere);
			}

			Plane testPlane = new Plane(
				new Vector3(0, -3, 5f), 
				new Vector3(0, 0.6f, 1), 
				new Vector3(1, -0.3f, 0),
				3f,
				1.5f
				);
			testPlane.material.color = new RaytracingColor(0.6f, 0.7f, 0.2f);
			this.scene.AddObject(testPlane);

			// Back wall (negative Z direction)
			Level backWall = new Level(
				new Vector3(origin.X, origin.Y, origin.Z + length),
				Vector3.UnitY,
				Vector3.UnitX
			);
			backWall.material.color = new RaytracingColor(1, 1, 1);
			this.scene.AddObject(backWall);
			
			// Left wall
			Level leftWall = new Level(
				new Vector3(origin.X - width / 2, origin.Y, origin.Z),
				Vector3.UnitY,
				Vector3.UnitZ
			);
			leftWall.material.color = new RaytracingColor(1, 0, 0);
			this.scene.AddObject(leftWall);

			// Right wall
			Level rightWall = new Level(
				new Vector3(origin.X + width / 2, origin.Y, origin.Z),
				Vector3.UnitZ,
				Vector3.UnitY
			);
			rightWall.material.color = new RaytracingColor(0, 0, 1);
			this.scene.AddObject(rightWall);
			
			// Ceiling
			Level ceiling = new Level(
				new Vector3(origin.X - width / 2, origin.Y + height / 2, origin.Z),
				Vector3.UnitX,
				Vector3.UnitZ
			);
			ceiling.material.color = new RaytracingColor(0, 1, 0);
			this.scene.AddObject(ceiling);
			
			// Floor
			Level floor = new Level(
				new Vector3(origin.X - width / 2, origin.Y - height / 2, origin.Z),
				Vector3.UnitZ,
				Vector3.UnitX
			);
			floor.material.color = new RaytracingColor(0.5f, 0.5f, 0.5f);
			this.scene.AddObject(floor);
							
			this.RenderFrame();
		}


		private void RenderFrame() {
			int width = frame.PixelWidth;
			int height = frame.PixelHeight;
			int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {

					Ray ray = this.scene.camera.GetRay(x, y, width, height);
					RayHit hit = this.scene.TraceRay(ray);

					if (hit.HasHit) {
                        int index = (y * stride) + (x * 4);
                        RaytracingColor color = new RaytracingColor(0, 0, 0) + this.diffuseColor;
                        
						foreach (var light in this.scene.Lights)
						{
							Vector3 toLight = light.transform.Position - hit.Position!.Value;
							toLight = toLight.Normalized;  // Normalize the light direction
							
							float dotProduct = Vector3.Dot(toLight, hit.Normal!.Value);
							if (dotProduct > 0)  // Ensure light is in front of the surface
							{
								color += (hit.HitObject.material?.color ?? new RaytracingColor(1, 1, 1)) 
										 * light.color 
										 * dotProduct;
							}
						}

                        var col = color.ToColor();
                        
                        pixels[index] = col.B;     // Blue
                        pixels[index + 1] = col.G; // Green
                        pixels[index + 2] = col.R; // Red
                        pixels[index + 3] = 255;        // Alpha
					}
				}
			}

			this.frame.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixels, stride, 0);
		}

	}

}