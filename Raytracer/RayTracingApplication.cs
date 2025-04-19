using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Raytracer.Core;
using Raytracer.Core.Objects;
using Raytracer.Core.Materials;
using Raytracer.Core.Math;
using Color = Raytracer.Core.Color;

namespace Raytracer;

public class RayTracingApplication {

    private WriteableBitmap frame;
    private Image screen;
    private Scene scene;

    private int width;
    private int height;
    private int stride;

    private const float SHADOW_RAY_EPSILON = 1e-4f;
    private const float SURFACE_EPSILON = 1e-4f;
    private const int REFLECT_RECURSION_DEPTH = 6;
    private const float AIR_REFRACTIVE_INDEX = 1.0f;

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

        Plane plane = new Plane(30, 60);
        plane.transform.MoveTo(0, -5f, 3);
        plane.material = MaterialLibrary.WhitePlastic;
        this.scene.AddObject(plane);
        
        Plane leftWall = new Plane(20, 60);
        leftWall.transform.MoveTo(-15, -5f, 3);
        leftWall.transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, -(float)Math.PI / 2f);
        leftWall.material = MaterialLibrary.CyanPlastic;
        this.scene.AddObject(leftWall);

        Cube cube = new Cube(2f);
        cube.transform.MoveTo(0f, 0f, 5f);
        cube.transform.rotation = Quaternion.FromAxisAngle(new Vector3(1, 1, 0), -(float)Math.PI / 8);
        cube.material = MaterialLibrary.Glass;
        this.scene.AddObject(cube);

        Sphere sphere = new Sphere(3);
        sphere.transform.MoveTo(3, -0.75f, 12);
        sphere.material = MaterialLibrary.Silver;
        this.scene.AddObject(sphere);
        
        Sphere sphere2 = new Sphere(2);
        sphere2.transform.MoveTo(-5, -0.75f, 15);
        sphere2.material = MaterialLibrary.Ruby;
        this.scene.AddObject(sphere2);
        
        Task.Run(() => Animate(cube));
    }

    private void Animate(Cube cube) {
        int i = 0;
        double piOver32 = Math.PI / 32;

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
            
            // Apply gamma correction for better visual results
            color = ApplyGammaCorrection(color);
            
            int index = (y * stride) + (x * 4);
            var col = color.ToSystemColor();
            pixels[index] = col.B;
            pixels[index + 1] = col.G;
            pixels[index + 2] = col.R;
            pixels[index + 3] = 255;
        }
    }
    
    // Apply a simple gamma correction for better visual results
    private Color ApplyGammaCorrection(Color color) {
        float gamma = 1.0f / 2.2f; // Standard gamma correction value
        return new Color(
            (float)Math.Pow(color.R, gamma),
            (float)Math.Pow(color.G, gamma),
            (float)Math.Pow(color.B, gamma)
        );
    }

    // Recursive method to trace rays and handle reflections and refractions
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
        Vector3 hitPoint = hit.Position!.Value;
        Vector3 normal = hit.Normal!.Value;

        // Make sure normal is pointing against the incident ray
        bool entering = Vector3.Dot(ray.Direction, normal) < 0;
        if (!entering) {
            normal = -normal;
        }

        // Start with ambient lighting
        Color resultColor = material.Ambient;

        // Direct illumination is only significant for opaque objects
        float opacityFactor = 1.0f - material.Transparency;
        
        // Calculate direct illumination with diffuse and specular components
        foreach (var light in scene.Lights) {
            Vector3 toLight = (light.transform.position - hitPoint);
            float distanceToLight = toLight.Magnitude;
            Vector3 toLightNormalized = toLight / distanceToLight; // Normalize
            
            // Check if something blocks the path to the light
            Vector3 shadowRayOrigin = hitPoint + normal * SURFACE_EPSILON;
            Ray shadowRay = new Ray(shadowRayOrigin, toLightNormalized);
            RayHit shadowHit = scene.TraceRay(shadowRay);

            float lightContribution = 1.0f;
            if (shadowHit.HasHit && shadowHit.Distance < distanceToLight) {
                // If the blocking object is transparent, allow some light through
                if (shadowHit.HitObject.material.Transparency > 0) {
                    lightContribution = shadowHit.HitObject.material.Transparency;
                } else {
                    lightContribution = 0;
                }
            }

            if (lightContribution > 0) {
                // Clear path to the light - add diffuse lighting
                float diffuseFactor = Math.Max(Vector3.Dot(toLightNormalized, normal), 0);
                resultColor += material.Diffuse * light.color * diffuseFactor * lightContribution * opacityFactor;
                
                // Add specular highlights (Phong model)
                Vector3 reflectedLight = CalculateReflection(-toLightNormalized, normal);
                Vector3 toCam = -ray.Direction;
                float specularFactor = Math.Max(Vector3.Dot(reflectedLight, toCam), 0);
                float specularPower = 32.0f;
                if (specularFactor > 0) {
                    specularFactor = (float)Math.Pow(specularFactor, specularPower);
                    resultColor += material.Specular * light.color * specularFactor * lightContribution;
                }
            }
        }

        // Calculate Fresnel reflectance
        float n1 = entering ? AIR_REFRACTIVE_INDEX : material.RefractiveIndex;
        float n2 = entering ? material.RefractiveIndex : AIR_REFRACTIVE_INDEX;
        float fresnelReflectance = CalculateFresnelReflectance(ray.Direction, normal, n1, n2);
        
        // Handle reflection and refraction
        Color reflectionColor = new Color(0, 0, 0);
        Color refractionColor = new Color(0, 0, 0);
        
        // Handle refraction
        if (material.Transparency > 0 && depth < REFLECT_RECURSION_DEPTH) {
            if (TryCalculateRefraction(ray.Direction, normal, n1, n2, out Vector3 refractionDir)) {
                // Properly offset refraction ray origin
                Vector3 refractionOrigin = hitPoint - normal * SURFACE_EPSILON;
                Ray refractionRay = new Ray(refractionOrigin, refractionDir);
                
                // Trace refraction ray
                refractionColor = TraceRay(refractionRay, depth + 1);
            }
        }
        
        // Handle reflection
        if ((material.Reflectivity > 0 || fresnelReflectance > 0) && depth < REFLECT_RECURSION_DEPTH) {
            Vector3 reflectionDir = CalculateReflection(ray.Direction, normal);
            Vector3 reflectionOrigin = hitPoint + normal * SURFACE_EPSILON;
            Ray reflectionRay = new Ray(reflectionOrigin, reflectionDir);
            
            // Trace reflection ray
            reflectionColor = TraceRay(reflectionRay, depth + 1);
        }
        
        // Blend reflection and refraction based on material properties and Fresnel
        float effectiveReflectivity;
        if (material.Transparency > 0) {
            // For transparent materials, use Fresnel equation to determine reflection amount
            effectiveReflectivity = fresnelReflectance;
        } else {
            // For opaque materials, use the material's reflectivity
            effectiveReflectivity = material.Reflectivity;
        }
        
        // Final color calculation
        if (material.Transparency > 0) {
            // For transparent materials, blend refraction and reflection
            float refractionWeight = material.Transparency * (1 - effectiveReflectivity);
            float reflectionWeight = effectiveReflectivity;
            float surfaceWeight = 1.0f - refractionWeight - reflectionWeight;
            
            resultColor = resultColor * surfaceWeight + 
                         refractionColor * refractionWeight + 
                         reflectionColor * reflectionWeight;
        } else {
            // For opaque materials, blend surface and reflection
            resultColor = resultColor * (1 - effectiveReflectivity) + 
                         reflectionColor * effectiveReflectivity;
        }
        
        return resultColor;
    }

    // Helper method to calculate reflection direction
    private Vector3 CalculateReflection(Vector3 incident, Vector3 normal) {
        return incident - 2 * Vector3.Dot(normal, incident) * normal;
    }

    // Helper method to calculate Fresnel reflectance
    private float CalculateFresnelReflectance(Vector3 incident, Vector3 normal, float n1, float n2) {
        // Ensure the incident vector is normalized
        incident = incident.Normalized;
        
        // Calculate the cosine of the incident angle (clamped to avoid numerical errors)
        float cosi = Math.Clamp(Vector3.Dot(-incident, normal), -1.0f, 1.0f);
        
        // Calculate the sine squared of the transmission angle using Snell's law
        float n_ratio = n1 / n2;
        float sint2 = n_ratio * n_ratio * (1.0f - cosi * cosi);
        
        // Check for total internal reflection
        if (sint2 > 1.0f) {
            return 1.0f;
        }
        
        // Calculate the cosine of the transmission angle
        float cost = (float)Math.Sqrt(1.0f - sint2);
        cosi = Math.Abs(cosi);
        
        // Calculate the Fresnel reflectance using Fresnel equations
        float Rs = ((n2 * cosi) - (n1 * cost)) / ((n2 * cosi) + (n1 * cost));
        float Rp = ((n1 * cosi) - (n2 * cost)) / ((n1 * cosi) + (n2 * cost));
        
        // Return the average of the two polarization components
        return (Rs * Rs + Rp * Rp) / 2.0f;
    }

    // Helper method to calculate refraction direction using Snell's law
    private bool TryCalculateRefraction(Vector3 incident, Vector3 normal, float n1, float n2, out Vector3 refracted) {
        // Ensure the incident vector is normalized
        incident = incident.Normalized;
        
        float n = n1 / n2;
        float cosI = Vector3.Dot(-incident, normal);
        float sint2 = n * n * (1.0f - cosI * cosI);
        
        if (sint2 > 1.0f) {
            // Total internal reflection
            refracted = Vector3.Zero;
            return false;
        }
        
        float cosT = (float)Math.Sqrt(1.0f - sint2);
        refracted = n * incident + (n * cosI - cosT) * normal;
        refracted = refracted.Normalized; // Ensure direction is normalized
        return true;
    }
}