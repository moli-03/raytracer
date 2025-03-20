using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        private const float SHADOW_RAY_EPSILON = 1e-4f;

        public RayTracingApplication(Image screen) {
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
            light.transform.MoveTo(-3, 2.5f, 0);
            this.scene.AddLight(light);

            Plane plane = new Plane(20, 60);
            plane.transform.MoveTo(0, -5f, 3);
            plane.material.color = new RaytracingColor(0, 0.35f, 0.75f);
            this.scene.AddObject(plane);

            Cube cube = new Cube(1f);
            cube.transform.MoveTo(-0.5f, 1.5f, 5);
            cube.transform.rotation = Quaternion.FromAxisAngle(new Vector3(1, 1, 0), -(float)Math.PI / 8);
            cube.material.color = new RaytracingColor(0, 0.75f, 0.35f);
            this.scene.AddObject(cube);

            Sphere sphere = new Sphere(3);
            sphere.transform.MoveTo(3, -0.75f, 12);
            sphere.material.color = new RaytracingColor(0.6f, 0.35f, 0.25f);
            this.scene.AddObject(sphere);
            
            Task.Run(() => Animate(cube));
        }

        private void Animate(Cube cube) {
            int i = 0;
            double piOver8 = Math.PI / 8;

            while (true) {
                i++;
                DateTime start = DateTime.Now;

                byte[] pixels = RenderFrame();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    frame.Lock();
                    frame.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
                    frame.Unlock();
                });

                Console.WriteLine("Render duration: " + (DateTime.Now - start).TotalMilliseconds + "ms");
                
                cube.transform.rotation = Quaternion.FromAxisAngle(new Vector3(0.25f, 1f, 0.5f), (float)(piOver8 * i));
                float red = (float)(Math.Sin(piOver8 * i) + 1) / 2;
                float green = (float)(Math.Sin(piOver8 * i + Math.PI / 2) + 1) / 2;
                float blue = (float)(Math.Sin(piOver8 * i + Math.PI) + 1) / 2;
                cube.material.color = new RaytracingColor(red, green, blue);
            }
        }

        private byte[] RenderFrame() {
            byte[] pixels = new byte[height * stride];
            Parallel.For(0, height, y => RenderLine(y, pixels));
            return pixels;
        }

        private void RenderLine(int y, byte[] pixels) {
            for (int x = 0; x < width; x++) {
                Ray ray = scene.camera.GetRay(x, y, width, height);
                RayHit hit = scene.TraceRay(ray);

                if (hit.HasHit) {
                    int index = (y * stride) + (x * 4);
                    RaytracingColor color = new RaytracingColor(0, 0, 0) + diffuseColor;

                    foreach (var light in scene.Lights) {
                        Vector3 toLight = (light.transform.position - hit.Position!.Value);
                        Vector3 toLightNormalized = toLight.Normalized;
                        
                        // Check if something blocks the path to the light
                        Vector3 shadowRayOrigin = hit.Position.Value + toLightNormalized * SHADOW_RAY_EPSILON;
                        Ray shadowRay = new Ray(shadowRayOrigin, toLightNormalized);
                        RayHit shadowHit = scene.TraceRay(shadowRay);

                        if (shadowHit.HasHit && shadowHit.Distance < toLight.Magnitude)
                        {
                            // Something is in the way -> no color
                        }
                        else
                        {
                            // Clear path to the light
                            float dot = Math.Max(Vector3.Dot(toLightNormalized, hit.Normal!.Value), 0);
                            color += (hit.HitObject.material?.color ?? new RaytracingColor(1, 1, 1)) * light.color * dot;
                        }
                    }

                    var col = color.ToColor();
                    pixels[index] = col.B;
                    pixels[index + 1] = col.G;
                    pixels[index + 2] = col.R;
                    pixels[index + 3] = 255;
                }
            }
        }
    }
}
