using Raytracer.Core.Materials;
using Raytracer.Core.Math;
using Raytracer.Core.Objects;

namespace Raytracer.Core.Scenes;

public class MinimalScene : BaseScene
{
    public override string Name => "Minimal Scene";

    private Sphere centralSphere;
    
    public override void Initialize(Scene scene)
    {
        Console.WriteLine($"Initializing {Name}...");
        
        // Setup camera
        SetupCamera(scene, new Vector3(0, 2, -8), new Vector3(0, 0, 1));
        
        // Add a single light
        Light light = new Light(new Color(1, 1, 1));
        light.transform.MoveTo(5, 5, -5);
        scene.AddLight(light);
        
        // Add a floor
        Plane floor = new Plane(20, 20);
        floor.transform.MoveTo(0, -1, 5);
        floor.material = MaterialLibrary.WhitePlastic;
        scene.AddObject(floor);
        
        // Add a central sphere that will be animated
        centralSphere = new Sphere(2);
        centralSphere.transform.MoveTo(0, 2, 5);
        centralSphere.material = MaterialLibrary.Emerald;
        scene.AddObject(centralSphere);
        
        Console.WriteLine($"Scene initialized with {scene.Objects.Count} objects and {scene.Lights.Count} lights");
    }

    public override void Update(Scene scene, int frameCount, float deltaTime)
    {
        // Bounce the sphere up and down
        float time = frameCount * 0.1f;
        float bounceHeight = (float)System.Math.Abs(System.Math.Sin(time)) * 2.0f;
        
        centralSphere.transform.MoveTo(0, 1 + bounceHeight, 5);
        
        // Rotate the camera around the scene
        float cameraAngle = time * 0.2f;
        float cameraRadius = 8.0f;
        
        float cameraX = (float)System.Math.Cos(cameraAngle) * cameraRadius;
        float cameraZ = (float)System.Math.Sin(cameraAngle) * cameraRadius - 3.0f;
        
        scene.camera.Position = new Vector3(cameraX, 2, cameraZ);
        scene.camera.LookDirection = new Vector3(0, 1, 5) - scene.camera.Position;
    }
}
