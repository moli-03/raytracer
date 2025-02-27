using System.Diagnostics;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Raytracer.Core;

namespace Raytracer {

	public class RayTracingApplication {

		private WriteableBitmap frame;
		private Image screen;
		private Scene scene;

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
			
			List<Sphere> balls = new List<Sphere>();
			balls.Add(new Sphere(2));
			balls.Add(new Sphere(2));
			
			foreach (var sphere in balls)
			{
				sphere.material.color = new RaytracingColor(0, 0.75f, 0.75f);
			}
			
			balls[0].transfrom.MoveTo(-1.5f, -1, 6);
			balls[1].transfrom.MoveTo(1.5f, -1, 6);
			
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
			shaft.Add(new Sphere(2));
			shaft.Add(new Sphere(2));

			for (int i = 0; i < shaft.Count; i++)
			{
				shaft[i].transfrom.MoveTo(0, i * 0.5f, 6);
				shaft[i].material.color = new RaytracingColor(0, 1f * i / shaft.Count, 0.75f);
				this.scene.AddObject(shaft[i]);
			}

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
                        RaytracingColor color = hit.HitObject.material.color;
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