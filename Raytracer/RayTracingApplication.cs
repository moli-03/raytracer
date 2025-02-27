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

			Sphere blue = new Sphere(2);
			blue.transfrom.MoveTo(0, 2, 10);
			blue.material.color = new RaytracingColor(0, 0, 1);
			
			Sphere green = new Sphere(2);
			green.transfrom.MoveTo(1.5f, 0, 10);
			green.material.color = new RaytracingColor(0, 1, 0);
			
			Sphere red = new Sphere(2);
			red.transfrom.MoveTo(-1.5f, 0, 10);
			red.material.color = new RaytracingColor(1, 0, 0);

			this.scene.AddObject(red);
			this.scene.AddObject(green);
			this.scene.AddObject(blue);

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
                        RaytracingColor color = hit.Color;
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