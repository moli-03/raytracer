using Raytracer.Core.Materials;
using Raytracer.Core.Math;
using Raytracer.Core.Objects;

namespace Raytracer.Core.Scenes;

public class SpheresScene : BaseScene
{
    public override string Name => "Spheres Scene";

    public override bool Animated => true;
    
    // Keep references to objects we want to animate
    private List<Sphere> animatedSpheres = new List<Sphere>();
    
    public override void Initialize(Scene scene)
    {
        Console.WriteLine($"Initializing {Name}...");
        
        // Setup camera with a wider view
        SetupCamera(scene, new Vector3(0, 5, -15), new Vector3(0, 0, 1), 60f);
        
        // Add lights
        Light mainLight = new Light(new Color(0.9f, 0.9f, 1.0f));
        mainLight.transform.MoveTo(5, 10, -5);
        scene.AddLight(mainLight);
        
        Light fillLight = new Light(new Color(0.4f, 0.4f, 0.6f));
        fillLight.transform.MoveTo(-8, 3, -3);
        scene.AddLight(fillLight);
        
        // Create a floor
        Plane floor = new Plane(50, 50);
        floor.transform.MoveTo(0, -2f, 10);
        floor.material = MaterialLibrary.WhitePlastic;
        scene.AddObject(floor);
        
        // Create a grid of spheres
        CreateSphereGrid(scene, 7, 7);
        
        Console.WriteLine($"Scene initialized with {scene.Objects.Count} objects and {scene.Lights.Count} lights");
    }

    public override void Update(Scene scene, int frameCount, float deltaTime)
    {
        // Animate the camera in a smooth arc
        float time = frameCount * 0.01f;
        float radius = 20.0f;
        float height = 5.0f + (float)System.Math.Sin(time * 0.5f) * 3.0f;
        
        float x = (float)System.Math.Cos(time) * radius;
        float z = (float)System.Math.Sin(time) * radius - 10.0f;
        
        scene.camera.Position = new Vector3(x, height, z);
        scene.camera.LookDirection = new Vector3(0, 0, 10) - scene.camera.Position;
        
        // Animate the spheres in a wave pattern
        AnimateSpheres(frameCount);
    }
    
    private void CreateSphereGrid(Scene scene, int rows, int cols)
    {
        // Different materials for variety
        var materials = new Material[] {
            MaterialLibrary.Ruby,
            MaterialLibrary.Gold,
            MaterialLibrary.Silver, 
            MaterialLibrary.Emerald,
            MaterialLibrary.Glass,
            MaterialLibrary.Chrome
        };
        
        float spacing = 3.0f;
        float startX = -(cols - 1) * spacing / 2;
        float startZ = 5.0f;
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float x = startX + col * spacing;
                float z = startZ + row * spacing;
                
                // Sphere size varies
                float radius = 0.4f + ((row + col) % 3) * 0.2f;
                
                Sphere sphere = new Sphere(radius);
                sphere.transform.MoveTo(x, -1.5f + radius, z);
                
                // Assign material based on position
                int materialIndex = (row + col) % materials.Length;
                sphere.material = materials[materialIndex];
                
                scene.AddObject(sphere);
                animatedSpheres.Add(sphere);
            }
        }
        
        Console.WriteLine($"Created grid of {rows * cols} spheres");
    }
    
    private void AnimateSpheres(int frameCount)
    {
        float time = frameCount * 0.05f;
        
        // Animate each sphere in a wave pattern
        for (int i = 0; i < animatedSpheres.Count; i++)
        {
            Sphere sphere = animatedSpheres[i];
            
            // Original position components
            float baseX = sphere.transform.position.X;
            float baseZ = sphere.transform.position.Z;
            
            // Calculate wave height based on position and time
            float waveHeight = (float)System.Math.Sin(baseX * 0.5f + time) * 
                               (float)System.Math.Cos(baseZ * 0.5f + time * 0.7f) * 1.0f;
            
            // Update position with the wave height
            Vector3 newPos = new Vector3(
                baseX,
                -1.5f + sphere.Radius + waveHeight,
                baseZ
            );
            
            sphere.transform.MoveTo(newPos);
        }
    }
}
