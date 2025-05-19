using Raytracer.Core.Math;

namespace Raytracer.Core.Scenes;

public abstract class BaseScene : IScene
{
    public abstract string Name { get; }
    
    public abstract void Initialize(Scene scene);
    
    public virtual void Update(Scene scene, int frameCount, float deltaTime)
    {
        // Default implementation does nothing - scenes can override this
    }
    
    // Helper methods for all scenes
    protected void SetupCamera(Scene scene, Vector3 position, Vector3 lookDirection, float fov = 75f)
    {
        scene.camera.Position = position;
        scene.camera.LookDirection = lookDirection;
        scene.camera.Fov = fov;
    }
}
