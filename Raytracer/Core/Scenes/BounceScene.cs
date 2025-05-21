using Raytracer.Core.Materials;
using Raytracer.Core.Math;
using Raytracer.Core.Objects;

namespace Raytracer.Core.Scenes;

public class BounceScene : BaseScene
{
    public override string Name => "Bouncing Spheres";

    public override bool Animated => true;
    
    private List<Sphere> spheres = new List<Sphere>();
    private List<float> speeds = new List<float>();
    private List<float> phases = new List<float>();

    public override void Initialize(Scene scene)
    {
        Console.WriteLine($"Initializing {Name}...");
        
        // Setup camera with slightly elevated position looking down at the scene
        SetupCamera(scene, new Vector3(0, 8, -12), new Vector3(0, -0.3f, 1));
        
        // Add lights
        Light mainLight = new Light(new Color(1.0f, 0.9f, 0.8f));
        mainLight.transform.MoveTo(10, 10, -5);
        scene.AddLight(mainLight);
        
        Light fillLight = new Light(new Color(0.4f, 0.5f, 0.8f));
        fillLight.transform.MoveTo(-8, 5, -3);
        scene.AddLight(fillLight);
        
        // Add a shiny floor
        Plane floor = new Plane(30, 30);
        floor.transform.MoveTo(0, -1, 5);
        floor.material = MaterialLibrary.Chrome;
        scene.AddObject(floor);
        
        // Create bouncing spheres with different sizes, materials, and bounce parameters
        CreateBouncingSpheres(scene);
        
        Console.WriteLine($"Scene initialized with {scene.Objects.Count} objects and {scene.Lights.Count} lights");
    }

    public override void Update(Scene scene, int frameCount, float deltaTime)
    {
        // Animate camera in circular path
        float cameraTime = frameCount * 0.005f;
        float cameraRadius = 15.0f;
        
        float cameraX = (float)System.Math.Sin(cameraTime) * cameraRadius;
        float cameraZ = (float)System.Math.Cos(cameraTime) * cameraRadius - 8.0f;
        
        scene.camera.Position = new Vector3(cameraX, 8, cameraZ);
        scene.camera.LookDirection = new Vector3(0, 0, 5) - scene.camera.Position;
        
        // Animate each sphere with its own bouncing pattern
        AnimateSpheres(frameCount);
    }
    
    private void CreateBouncingSpheres(Scene scene)
    {
        // Collection of materials to choose from
        var materials = new Material[] {
            MaterialLibrary.Ruby,
            MaterialLibrary.Gold,
            MaterialLibrary.Emerald,
            MaterialLibrary.Glass,
            MaterialLibrary.Chrome,
            MaterialLibrary.Silver
        };
        
        // Create several spheres with varying parameters
        for (int i = 0; i < 12; i++)
        {
            float angle = (float)i / 12 * (float)System.Math.PI * 2;
            float radius = 8.0f;
            
            // Calculate position in a circle
            float x = (float)System.Math.Cos(angle) * radius;
            float z = (float)System.Math.Sin(angle) * radius + 5.0f;
            
            // Sphere size varies
            float size = 0.5f + ((i % 3) * 0.4f);
            
            Sphere sphere = new Sphere(size);
            sphere.transform.MoveTo(x, size, z);
            
            // Assign material 
            int materialIndex = i % materials.Length;
            sphere.material = materials[materialIndex];
            
            // Add to scene and tracking collections
            scene.AddObject(sphere);
            spheres.Add(sphere);
            
            // Assign random bounce speed and phase
            speeds.Add(0.5f + (i % 5) * 0.2f);
            phases.Add(i * 0.5f);
        }
    }
    
    private void AnimateSpheres(int frameCount)
    {
        for (int i = 0; i < spheres.Count; i++)
        {
            Sphere sphere = spheres[i];
            float speed = speeds[i];
            float phase = phases[i];
            
            // Calculate bounce height using sine wave
            float time = frameCount * 0.05f * speed + phase;
            float bounceHeight = (float)System.Math.Abs(System.Math.Sin(time)) * 5.0f;
            
            // Get base position (x and z stay constant)
            float baseY = sphere.Radius; // Radius is the minimum height when sitting on ground
            
            // Update sphere position
            Vector3 pos = sphere.transform.position;
            sphere.transform.MoveTo(pos.X, baseY + bounceHeight, pos.Z);
        }
    }
}
