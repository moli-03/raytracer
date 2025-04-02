using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Raytracer.Core.Objects;
using Raytracer.Core.Objects.Materials;
using Color = Raytracer.Core.Objects.Color;

namespace Raytracer;

public class RayTracingApplication {

    private WriteableBitmap frame;
    private Image screen;
    private Scene scene;

    private int width;
    private int height;
    private int stride;

    private const float SHADOW_RAY_EPSILON = 1e-4f;
    private const int REFLECT_RECURSION_DEPTH = 3;

    public RayTracingApplication(Image screen) {
        this.scene = new Scene();
        this.screen = screen;
    
        // Get the actual dimensions from the screen element
        double screenWidth = screen.Width > 0 ? screen.Width : 500;
        double screenHeight = screen.Height > 0 ? screen.Height : 500;
    
        this.frame = new WriteableBitmap(
            (int)screenWidth, 
            (int)screenHeight, 
            96, 96, 
            PixelFormats.Bgr32, 
            null
        );
    
        this.screen.Source = this.frame;
        width = frame.PixelWidth;
        height = frame.PixelHeight;
        stride = width * 4;
    }
    
    public void Run() {
        Light light = new Light(new Color(1, 1, 1));
        light.transform.MoveTo(-3, 2.5f, 0);
        this.scene.AddLight(light);

        Plane plane = new Plane(20, 60);
        plane.transform.MoveTo(0, -5f, 3);
        plane.material = MaterialLibrary.WhitePlastic;
        this.scene.AddObject(plane);

        Cube cube = new Cube(2f);
        cube.transform.MoveTo(1.2f, 1.2f, 5);
        cube.transform.rotation = Quaternion.FromAxisAngle(new Vector3(1, 1, 0), -(float)Math.PI / 8);
        cube.material = MaterialLibrary.Gold;
        this.scene.AddObject(cube);

        Sphere sphere = new Sphere(3);
        sphere.transform.MoveTo(3, -0.75f, 12);
        sphere.material = MaterialLibrary.Silver;
        this.scene.AddObject(sphere);
        
        Sphere sphere2 = new Sphere(2);
        sphere2.transform.MoveTo(-3, -0.75f, 7);
        sphere2.material = MaterialLibrary.Emerald;
        this.scene.AddObject(sphere2);
        
        Task.Run(() => Animate(cube));
    }

    private void Animate(Cube cube) {
        int i = 0;
        double piOver32 = Math.PI / 32;
        double piOver64 = Math.PI / 64;

        var center = new Vector3(0, 0, 12f);

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
            
            cube.transform.rotation = Quaternion.FromAxisAngle(new Vector3(0.25f, 1f, 0.5f), (float)(piOver32 * i));

            float x = (float)Math.Cos(piOver32 * i) * 12f + center.X;
            float z = (float)Math.Sin(piOver32 * i) * 12f + center.Z;
            
            this.scene.camera.Position = new Vector3(x, 0, z);
            this.scene.camera.LookDirection = center - this.scene.camera.Position;
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
            
            // Get the color by tracing the ray (with recursion for reflections)
            Color color = TraceRay(ray, 0);
            
            int index = (y * stride) + (x * 4);
            var col = color.ToSystemColor();
            pixels[index] = col.B;
            pixels[index + 1] = col.G;
            pixels[index + 2] = col.R;
            pixels[index + 3] = 255;
        }
    }

    // Recursive method to trace rays and handle reflections
    private Color TraceRay(Ray ray, int depth) {
        // Check if we've exceeded the maximum recursion depth
        if (depth > REFLECT_RECURSION_DEPTH) {
            return new Color(0, 0, 0);
        }

        RayHit hit = scene.TraceRay(ray);

        if (!hit.HasHit) {
            return new Color(0, 0, 0); // Return background color
        }

        Material material = hit.HitObject.material;

        // Start with ambient lighting
        Color resultColor = material.Ambient;

        // Calculate direct illumination with diffuse and specular components
        foreach (var light in scene.Lights) {
            Vector3 toLight = (light.transform.position - hit.Position!.Value);
            float distanceToLight = toLight.Magnitude;
            Vector3 toLightNormalized = toLight.Normalized;
            
            // Check if something blocks the path to the light
            Vector3 shadowRayOrigin = hit.Position.Value + toLightNormalized * SHADOW_RAY_EPSILON;
            Ray shadowRay = new Ray(shadowRayOrigin, toLightNormalized);
            RayHit shadowHit = scene.TraceRay(shadowRay);

            if (shadowHit.HasHit && shadowHit.Distance < distanceToLight) {
                // Something is in the way -> no direct light contribution
            } else {
                // Clear path to the light - add diffuse lighting
                float diffuseFactor = Math.Max(Vector3.Dot(toLightNormalized, hit.Normal!.Value), 0);
                resultColor += material.Diffuse * light.color * diffuseFactor;
                
                // Add specular highlights (Phong model)
                Vector3 reflectedLight = CalculateReflection(-toLightNormalized, hit.Normal.Value);
                Vector3 toCam = -ray.Direction;
                float specularFactor = Math.Max(Vector3.Dot(reflectedLight, toCam), 0);
                float specularPower = 32.0f; // Adjust shininess
                if (specularFactor > 0) {
                    specularFactor = (float)Math.Pow(specularFactor, specularPower);
                    resultColor += material.Specular * light.color * specularFactor;
                }
            }
        }

        // Calculate reflection if the material has reflectivity
        if (material.Reflectivity > 0 && depth < REFLECT_RECURSION_DEPTH) {
            // Calculate reflection ray
            Vector3 reflectionDir = CalculateReflection(ray.Direction, hit.Normal!.Value);
            Vector3 reflectionOrigin = hit.Position!.Value + reflectionDir * SHADOW_RAY_EPSILON;
            Ray reflectionRay = new Ray(reflectionOrigin, reflectionDir);
            
            // Recursively trace the reflection ray
            Color reflectionColor = TraceRay(reflectionRay, depth + 1);
            
            // Blend the reflection with the direct lighting based on reflectivity
            resultColor = resultColor * (1 - material.Reflectivity) + reflectionColor * material.Reflectivity;
        }
        
        return resultColor;
    }

    // Helper method to calculate reflection direction
    private Vector3 CalculateReflection(Vector3 incident, Vector3 normal) {
        // R = I - 2(N·I)N where I is incident vector, N is normal
        return incident - 2 * Vector3.Dot(normal, incident) * normal;
    }
}