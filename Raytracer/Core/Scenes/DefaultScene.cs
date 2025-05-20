using System.IO;
using Raytracer.Core.Materials;
using Raytracer.Core.Math;
using Raytracer.Core.Objects;
using Raytracer.IO;

namespace Raytracer.Core.Scenes;

public class DefaultScene : BaseScene
{
    public override string Name => "Default Scene";

    public override bool Animated => true;
    
    // Assets path
    private static readonly string AssetsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");

    public override void Initialize(Scene scene)
    {
        Console.WriteLine($"Initializing {Name}...");
        
        // Set up camera
        SetupCamera(scene, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        
        // Add lights
        Light light = new Light(new Color(1, 1, 1));
        light.transform.MoveTo(-3, 2.5f, 0);
        scene.AddLight(light);
        Console.WriteLine($"Added main light at position: {light.transform.position}");

        // Add scene objects
        AddFloorAndWalls(scene);
        AddSword(scene);
        AddPrimitives(scene);
        
        Console.WriteLine($"Scene initialized with {scene.Objects.Count} objects and {scene.Lights.Count} lights");
        Console.WriteLine($"Camera position: {scene.camera.Position}, looking at: {scene.camera.LookDirection}");
    }

    public override void Update(Scene scene, int frameCount, float deltaTime)
    {
        // Orbital camera animation
        float piOver32 = (float)System.Math.PI / 32;
        var center = new Vector3(0, 0, 12f);
        
        float angle = piOver32 * frameCount;
        float x = (float)System.Math.Cos(angle) * 12f + center.X;
        float z = (float)System.Math.Sin(angle) * 12f + center.Z;
        
        scene.camera.Position = new Vector3(x, 0, z);
        scene.camera.LookDirection = center - scene.camera.Position;
    }

    private void AddFloorAndWalls(Scene scene)
    {
        Plane plane = new Plane(30, 60);
        plane.transform.MoveTo(0, -5f, 3);
        plane.material = MaterialLibrary.WhitePlastic;
        scene.AddObject(plane);
        Console.WriteLine($"Added floor plane at position: {plane.transform.position} with dimensions: 30x60");
        
        Plane leftWall = new Plane(20, 60);
        leftWall.transform.MoveTo(-15, -5f, 3);
        leftWall.transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, -(float)System.Math.PI / 2f);
        leftWall.material = MaterialLibrary.CyanPlastic;
        scene.AddObject(leftWall);
        Console.WriteLine($"Added left wall at position: {leftWall.transform.position} with dimensions: 20x60");
    }
    
    private void AddSword(Scene scene)
    {
        try {
            Console.WriteLine("Loading sword model...");
            string objFilePath = Path.Combine(AssetsDirectory, "CaeraSword.obj");
            
            if (!File.Exists(objFilePath)) {
                Console.WriteLine($"ERROR: Sword model file not found at {objFilePath}");
            } else {
                Mesh sword = ObjLoader.LoadFromFile(objFilePath);
                
                // Move the sword further away to avoid intersection issues
                sword.transform.MoveTo(-2f, 0f, 10f);
                sword.transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitY, (float)System.Math.PI);
                // Scale down more to ensure it's not too large
                sword.transform.Scale = new Vector3(0.1f, 0.1f, 0.1f);
                sword.material = MaterialLibrary.Ruby;
                
                Console.WriteLine($"Sword loaded with {sword.GetTriangleCount()} triangles");
                scene.AddObject(sword);
                Console.WriteLine("Sword added to scene");
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error loading sword: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
    
    private void AddPrimitives(Scene scene)
    {
        Cube cube = new Cube(2);
        cube.transform.MoveTo(0, 0, 15);
        cube.material = MaterialLibrary.Gold;
        scene.AddObject(cube);
        Console.WriteLine($"Added cube at position: {cube.transform.position} with size: 2");

        Sphere sphere = new Sphere(3);
        // Move sphere slightly to avoid overlap with the sword/cube
        sphere.transform.MoveTo(4, -0.75f, 12);
        sphere.material = MaterialLibrary.Silver;
        scene.AddObject(sphere);
        Console.WriteLine($"Added sphere at position: {sphere.transform.position} with radius: 3");
        
        Sphere sphere2 = new Sphere(2);
        sphere2.transform.MoveTo(-5, -0.75f, 15);
        sphere2.material = MaterialLibrary.Ruby;
        scene.AddObject(sphere2);
        Console.WriteLine($"Added sphere2 at position: {sphere2.transform.position} with radius: 2");
    }
}
