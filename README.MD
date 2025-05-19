# Raytracer in C#

A real-time raytracer implemented in C# using WPF for display.

## Features

- Real-time rendering with reflections and refractions
- Multiple scene support with easy switching
- Animated scenes with custom animations
- Physically-based materials with reflectivity and transparency
- Support for various 3D primitives (spheres, planes, cubes)
- OBJ model loading

## Scene System

The raytracer includes a flexible scene system that allows for:

- Multiple predefined scenes
- Runtime scene switching
- Custom animations per scene
- Easy creation of new scenes

### Available Scenes

1. **Default Scene** - A showcase of various primitives and a loaded 3D model
2. **Spheres Scene** - A grid of animated spheres with a wave-like movement
3. **Minimal Scene** - A simple scene with a bouncing sphere for testing

### Controls

- Press `1-3` to switch between scenes
- Press `Space` to restart the current animation

## How to Create a New Scene

To create a new scene, create a class that inherits from `BaseScene` and implement
the required methods:

```csharp
public class MyCustomScene : BaseScene
{
    public override string Name => "My Custom Scene";
    
    public override void Initialize(Scene scene)
    {
        // Setup camera, lights, and objects
    }
    
    public override void Update(Scene scene, int frameCount, float deltaTime)
    {
        // Update the scene for animation
    }
}
```

Then register your scene in the `RayTracingApplication` constructor:

```csharp
this.sceneManager.RegisterScene(new MyCustomScene());
```