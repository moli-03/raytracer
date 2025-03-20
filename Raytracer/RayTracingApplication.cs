using System.ComponentModel;
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

		private int width;
		private int height;
		private int stride;

		private float diffuseColor = 0.1f;

		public RayTracingApplication(Image screen)
		{
			this.scene = new Scene();
			this.screen = screen;
            this.frame = new WriteableBitmap(500, 500, 96, 96, PixelFormats.Bgr32, null);
			this.screen.Source = this.frame;
			
			width = frame.PixelWidth;
			height = frame.PixelHeight;
			stride = width * 4;
		}

		public void Run() {
			
			Light light = new Light(new RaytracingColor(1, 1, 1));
			light.transform.MoveTo(0, 0, 0);
			this.scene.AddLight(light);
			
			var random = new Random();

			Triangle triangle = new Triangle(
				new Vector3(-1, -1, 0),
				new Vector3(0, 1, 0),
				new Vector3(1, -1, 0)
			);
			triangle.transform.MoveTo(-1, -1, 4);
			triangle.transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitX, (float)-Math.PI / 4);
			triangle.material.color = new RaytracingColor(0, 0.75f, 0.75f);
			this.scene.AddObject(triangle);

			Plane plane = new Plane(3, 4);
			plane.transform.MoveTo(0, -2, 3);
			plane.material.color = new RaytracingColor(0, 0.35f, 0.75f);
			this.scene.AddObject(plane);

			Cube cube = new Cube(2);
			cube.transform.MoveTo(1, -0.75f, 5);
			cube.transform.rotation = Quaternion.FromAxisAngle(new Vector3(1, 1, 0), -(float)Math.PI / 8);
			cube.material.color = new RaytracingColor(0, 0.75f, 0.35f);
			this.scene.AddObject(cube);

			var now = DateTime.Now;
			this.RenderFrame();
			Console.WriteLine("Render duration: " + (DateTime.Now - now).TotalMilliseconds + "ms");

			Task.Factory.StartNew(() =>
			{
				int i = 0;
				while (true)
				{
					i++;
					var pixels = this.RenderFrame();

					Application.Current.Dispatcher.Invoke(() =>
					{
						this.frame.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixels, stride, 0);
					});
					
					cube.transform.rotation = Quaternion.FromAxisAngle(new Vector3(0.25f, 1f, 0.5f), (float)Math.PI / 16 * i);
					float red = (float)(Math.Sin(Math.PI / 8 * i) + 1) / 2;
					float green = (float)(Math.Sin(Math.PI / 8 * i + Math.PI / 2) + 1) / 2;
					float blue = (float)(Math.Sin(Math.PI / 8 * i + Math.PI) + 1) / 2;
					cube.material.color = new RaytracingColor(red, green, blue);
				}
			});
		}


		private void RenderLines(int start, int end, int width, int height, int stride, byte[] pixels)
		{
			
			for (int y = start; y < end; y++) {
				for (int x = 0; x < width; x++) {

					Ray ray = this.scene.camera.GetRay(x, y, width, height);
					RayHit hit = this.scene.TraceRay(ray);

					if (hit.HasHit) {
                        int index = (y * stride) + (x * 4);
                        RaytracingColor color = new RaytracingColor(0, 0, 0) + this.diffuseColor;
                        
						foreach (var light in this.scene.Lights)
						{
							Vector3 toLight = light.transform.position - hit.Position!.Value;
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
		}

		private byte[] RenderFrame() {
			
			List<Task> tasks = new List<Task>();
			
            byte[] pixels = new byte[height * stride];
            
			int threads = 20;
			int linesPerThread = height / threads;
			for (int i = 0; i < threads; i++)
			{
				int start = i * linesPerThread;
				int end = i * linesPerThread + linesPerThread;

				if (i == threads - 1)
				{
					end = height;
				}
				
				tasks.Add(Task.Factory.StartNew(() =>
				{
					this.RenderLines(start, end, width, height, stride, pixels);
				}));
			}
			
			Task.WaitAll(tasks);

			return pixels;
		}

	}

}