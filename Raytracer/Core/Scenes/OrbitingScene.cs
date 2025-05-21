using Raytracer.Core.Materials;
using Raytracer.Core.Math;
using Raytracer.Core.Objects;

namespace Raytracer.Core.Scenes;

public class OrbitingScene : BaseScene
{
    public override string Name => "Orbiting Planets";

    public override bool Animated => true;
    
    private Sphere sun;
    private List<Sphere> planets = new List<Sphere>();
    private List<float> orbitRadii = new List<float>();
    private List<float> orbitSpeeds = new List<float>();
    private List<Vector3> orbitAxes = new List<Vector3>();

    public override void Initialize(Scene scene)
    {
        Console.WriteLine($"Initializing {Name}...");
        
        // Setup camera with good viewing angle
        SetupCamera(scene, new Vector3(0, 15, -20), new Vector3(0, -0.5f, 1));
        
        // Create the sun (emissive sphere in the center)
        CreateSun(scene);
        
        // Create orbiting planets
        CreatePlanets(scene);
        
        Console.WriteLine($"Scene initialized with {scene.Objects.Count} objects and {scene.Lights.Count} lights");
    }

    public override void Update(Scene scene, int frameCount, float deltaTime)
    {
        // Camera slowly orbits around the scene
        float cameraTime = frameCount * 0.003f;
        float cameraRadius = 25.0f;
        float cameraHeight = 15.0f + (float)System.Math.Sin(cameraTime * 0.5f) * 5.0f;
        
        float cameraX = (float)System.Math.Sin(cameraTime) * cameraRadius;
        float cameraZ = (float)System.Math.Cos(cameraTime) * cameraRadius - 5.0f;
        
        scene.camera.Position = new Vector3(cameraX, cameraHeight, cameraZ);
        scene.camera.LookDirection = new Vector3(0, 0, 0) - scene.camera.Position;
        
        // Animate planets orbiting the sun
        AnimatePlanets(frameCount, deltaTime);
    }
    
    private void CreateSun(Scene scene)
    {
        // Create a bright "sun" sphere in the center
        sun = new Sphere(3.0f);
        sun.transform.MoveTo(0, 0, 0);
        
        // Create an emissive material for the sun
        sun.material = MaterialLibrary.YellowPlastic;
        
        scene.AddObject(sun);
        
        // Add a light at the center to illuminate the scene
        Light sunLight = new Light(new Color(1.0f, 0.9f, 0.7f));
        sunLight.transform.MoveTo(0, 0, 0);
        scene.AddLight(sunLight);
    }
    
    private void CreatePlanets(Scene scene)
    {
        // Planet materials
        Material[] materials = new Material[]
        {
            MaterialLibrary.Ruby,
            MaterialLibrary.Gold,
            MaterialLibrary.Emerald,
            MaterialLibrary.Silver,
            MaterialLibrary.Chrome,
            MaterialLibrary.Glass
        };
        
        // Create planets at different distances from the sun
        for (int i = 0; i < 6; i++)
        {
            // Calculate initial position on orbit
            float orbitRadius = 6.0f + i * 3.0f;
            float orbitSpeed = 1.0f / (float)System.Math.Sqrt(orbitRadius) * 0.5f; // Kepler-like
            
            // Create slightly random orbit axis for variation
            Vector3 orbitAxis = Vector3.UnitY;
            if (i > 0)
            {
                float tilt = (i % 3) * 0.15f;
                orbitAxis = new Vector3(
                    (float)System.Math.Sin(tilt),
                    1.0f,
                    (float)System.Math.Sin(tilt + 1.0f)
                ).Normalized;
            }
            
            // Initial position
            float planetSize = 0.8f + (i % 3) * 0.4f;
            Sphere planet = new Sphere(planetSize);
            planet.transform.MoveTo(orbitRadius, 0, 0);
            planet.material = materials[i % materials.Length];
            
            // Store orbit parameters
            planets.Add(planet);
            orbitRadii.Add(orbitRadius);
            orbitSpeeds.Add(orbitSpeed);
            orbitAxes.Add(orbitAxis);
            
            scene.AddObject(planet);
        }
    }
    
    private void AnimatePlanets(int frameCount, float deltaTime)
    {
        for (int i = 0; i < planets.Count; i++)
        {
            Sphere planet = planets[i];
            float radius = orbitRadii[i];
            float speed = orbitSpeeds[i];
            Vector3 axis = orbitAxes[i];
            
            // Calculate angle based on speed and frame
            float angle = frameCount * 0.02f * speed;
            
            // Calculate orbit position - basic case with y-axis rotation
            float x = (float)System.Math.Cos(angle) * radius;
            float z = (float)System.Math.Sin(angle) * radius;
            
            // If we have a tilted orbit, apply rotation to the orbit position
            if (axis != Vector3.UnitY)
            {
                // Create a rotation around the tilted axis
                Quaternion rotation = Quaternion.FromAxisAngle(axis, angle);
                Vector3 basePos = new Vector3(radius, 0, 0);
                Vector3 rotated = rotation.Rotate(basePos);
                
                x = rotated.X;
                float y = rotated.Y;
                z = rotated.Z;
                
                planet.transform.MoveTo(x, y, z);
            }
            else
            {
                planet.transform.MoveTo(x, 0, z);
            }
        }
    }
}
