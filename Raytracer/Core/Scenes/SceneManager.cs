using System.Collections.Generic;

namespace Raytracer.Core.Scenes;

public class SceneManager
{
    private readonly Dictionary<string, IScene> scenes = new();
    private IScene? currentScene;
    private readonly Scene sceneInstance;
    
    public string? CurrentSceneName => currentScene?.Name;
    
    public SceneManager(Scene sceneInstance)
    {
        this.sceneInstance = sceneInstance;
    }
    
    public void RegisterScene(IScene scene)
    {
        if (!scenes.ContainsKey(scene.Name))
        {
            scenes.Add(scene.Name, scene);
            Console.WriteLine($"Scene registered: {scene.Name}");
        }
        else
        {
            Console.WriteLine($"Scene with name '{scene.Name}' already exists!");
        }
    }
    
    public bool SetActiveScene(string sceneName)
    {
        if (scenes.TryGetValue(sceneName, out IScene? scene))
        {
            // Clear existing scene
            sceneInstance.Clear();
            
            // Initialize the new scene
            currentScene = scene;
            currentScene.Initialize(sceneInstance);
            
            Console.WriteLine($"Activated scene: {sceneName}");
            return true;
        }
        
        Console.WriteLine($"Scene not found: {sceneName}");
        return false;
    }

    public IScene? GetCurrentScene()
    {
        return currentScene;
    }
    
    public void UpdateCurrentScene(int frameCount, float deltaTime)
    {
        currentScene?.Update(sceneInstance, frameCount, deltaTime);
    }
    
    public IEnumerable<string> GetAvailableScenes()
    {
        // Return scenes in registration order
        return scenes.Keys;
    }
}
