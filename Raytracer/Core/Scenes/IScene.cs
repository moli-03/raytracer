using Raytracer.Core.Math;

namespace Raytracer.Core.Scenes;

public interface IScene
{
    /// <summary>
    /// Unique name identifier for the scene
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Is the scene animated?
    /// </summary>
    bool Animated { get;  }
    
    /// <summary>
    /// Initialize the scene with all objects, lights, and camera settings
    /// </summary>
    /// <param name="scene">The scene object to populate</param>
    void Initialize(Scene scene);
    
    /// <summary>
    /// Update the scene for animation purposes
    /// </summary>
    /// <param name="scene">The scene to update</param>
    /// <param name="frameCount">Current frame number</param>
    /// <param name="deltaTime">Time elapsed since last frame in seconds</param>
    void Update(Scene scene, int frameCount, float deltaTime);
}
