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

			Sphere sphere = new Sphere(2);
			sphere.Transform.MoveTo(0, 0, 30);
			sphere.Material.color = System.Drawing.Color.Cyan;

			this.scene.AddObject(sphere);

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
						System.Drawing.Color color = hit.HitObject.Material.color;

                        pixels[index] = color.B;     // Blue
                        pixels[index + 1] = color.G; // Green
                        pixels[index + 2] = color.R; // Red
                        pixels[index + 3] = 255;        // Alpha
					}
				}
			}

			this.frame.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixels, stride, 0);
		}

	}

}