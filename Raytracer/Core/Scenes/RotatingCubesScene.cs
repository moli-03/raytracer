using Raytracer.Core.Materials;
using Raytracer.Core.Math;
using Raytracer.Core.Objects;

namespace Raytracer.Core.Scenes;

public class RotatingCubesScene : BaseScene
{
    public override string Name => "Rotating Cubes";

    public override bool Animated => true;
    
    private List<Cube> cubes = new List<Cube>();
    private List<Vector3> rotationAxes = new List<Vector3>();
    private List<float> rotationSpeeds = new List<float>();

    public override void Initialize(Scene scene)
    {
        Console.WriteLine($"Initializing {Name}...");
        
        // Setup camera with elevated position
        SetupCamera(scene, new Vector3(0, 7, 1), new Vector3(0, -0.8f, 1));
        
        // Add lights
        Light mainLight = new Light(new Color(1.0f, 0.98f, 0.95f));
        mainLight.transform.MoveTo(10, 15, -10);
        scene.AddLight(mainLight);
        
        Light fillLight = new Light(new Color(0.3f, 0.3f, 0.5f));
        fillLight.transform.MoveTo(-8, 8, -5);
        scene.AddLight(fillLight);
        
        // Add a reflective floor
        Plane floor = new Plane(40, 40);
        floor.transform.MoveTo(0, -2, 10);
        floor.material = MaterialLibrary.Chrome;
        scene.AddObject(floor);
        
        // Create pattern of rotating cubes
        CreateCubePattern(scene);
        
        Console.WriteLine($"Scene initialized with {scene.Objects.Count} objects and {scene.Lights.Count} lights");
    }

    public override void Update(Scene scene, int frameCount, float deltaTime)
    {
        // Camera slowly moves in figure-8 pattern
        float cameraTime = frameCount * 0.01f;
        float cameraX = (float)System.Math.Sin(cameraTime) * 4.0f;
        float cameraZ = (float)System.Math.Sin(cameraTime * 2) * 2.0f;
        
        // scene.camera.Position = new Vector3(cameraX, 7, cameraZ);
        // scene.camera.LookDirection = new Vector3(0, 0, 10) - scene.camera.Position;
        
        // Animate cube rotations
        AnimateCubes(frameCount);
    }
    
    private void CreateCubePattern(Scene scene)
    {
        // Materials for the cubes
        var materials = new Material[]
        {
            MaterialLibrary.Ruby,
            MaterialLibrary.Gold,
            MaterialLibrary.Emerald,
            MaterialLibrary.Silver,
            MaterialLibrary.Chrome
        };
        
        // Create a spiral pattern of cubes
        int numCubes = 7;
        float spiralRadius = 3f;
        float spiralHeight = 4f;
        
        for (int i = 0; i < numCubes; i++)
        {
            float angle = i * 0.4f;
            float radius = spiralRadius * angle;
            float x = (float)System.Math.Cos(angle) * radius;
            float z = (float)System.Math.Sin(angle) * radius + 10.0f;
            float y = i * spiralHeight;
            
            float size = 0.8f + (float)System.Math.Sin(i * 0.5f) * 0.3f;
            
            Cube cube = new Cube(size);
            cube.transform.MoveTo(x, y, z);
            
            // Assign material
            int materialIndex = i % materials.Length;
            cube.material = materials[materialIndex];
            
            // Generate random rotation axis and speed
            Vector3 rotAxis = new Vector3(
                (float)System.Math.Sin(i * 0.7f),
                (float)System.Math.Cos(i * 0.3f),
                (float)System.Math.Sin(i * 0.5f)
            ).Normalized;
            
            float rotSpeed = 2f + (i % 5) *6f;
            
            // Store the cube and its animation parameters
            cubes.Add(cube);
            rotationAxes.Add(rotAxis);
            rotationSpeeds.Add(rotSpeed);
            
            scene.AddObject(cube);
        }
    }
    
    private void AnimateCubes(int frameCount)
    {
        for (int i = 0; i < cubes.Count; i++)
        {
            Cube cube = cubes[i];
            Vector3 axis = rotationAxes[i];
            float speed = rotationSpeeds[i];
            
            // Calculate rotation angle
            float angle = frameCount * 0.01f * speed;
            
            // Apply rotation
            cube.transform.rotation = Quaternion.FromAxisAngle(axis, angle);
            
            // Add some vertical motion
            float baseY = i * 0.2f;
            float offsetY = (float)System.Math.Sin(frameCount * 0.02f + i * 0.2f) * 0.5f;
            Vector3 pos = cube.transform.position;
            cube.transform.MoveTo(pos.X, baseY + offsetY, pos.Z);
        }
    }
}
