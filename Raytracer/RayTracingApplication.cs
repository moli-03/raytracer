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

			/**
			Sphere big = new Sphere(4);
			big.transfrom.MoveTo(0, 0, 8);
			big.material.color = new RaytracingColor(0, 0, 1);
			
			Sphere small = new Sphere(2);
			small.transfrom.MoveTo(-1f, 1, 6);
			small.material.color = new RaytracingColor(0, 0.75f, 0.75f);
			

			this.scene.AddObject(big);
			this.scene.AddObject(small);
			**/

			Light light = new Light(new RaytracingColor(1, 1, 1));
			light.transform.MoveTo(-4, 8, 2);
			this.scene.AddLight(light);
			
			List<Sphere> balls = new List<Sphere>();
			balls.Add(new Sphere(2));
			balls.Add(new Sphere(2));
			
			var random = new Random();
			
			foreach (var sphere in balls)
			{
				sphere.material.color = new RaytracingColor(random.NextSingle(), random.NextSingle(), random.NextSingle());
			}
			
			balls[0].transform.MoveTo(-1.5f, -3, 6);
			balls[1].transform.MoveTo(1.5f, -3, 6);
			
			foreach (var sphere in balls)
			{
				this.scene.AddObject(sphere);
			}
			
			List<Sphere> shaft = new List<Sphere>();
			shaft.Add(new Sphere(2));
			shaft.Add(new Sphere(2));
			shaft.Add(new Sphere(2));
			shaft.Add(new Sphere(2));
			shaft.Add(new Sphere(2));
			shaft.Add(new Sphere(2));

			for (int i = 0; i < shaft.Count; i++)
			{
				shaft[i].transform.MoveTo(0, i * 0.85f - 2, 6);
				shaft[i].material.color = new RaytracingColor(random.NextSingle(), random.NextSingle(), random.NextSingle());
				this.scene.AddObject(shaft[i]);
			}

			// Fancy light animation
			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(15);
			float x = 0;
			timer.Tick += (s, e) =>
			{
				x += 0.25f;
				light.transform.MoveTo((float)Math.Cos(x) * 5f, 4, (float)Math.Sin(x) * 5f);
				foreach (var obj in this.scene.Objects)
				{
					obj.material.color = new RaytracingColor(random.NextSingle(), random.NextSingle(), random.NextSingle());
				}
				this.RenderFrame();
			};
			timer.Start();
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