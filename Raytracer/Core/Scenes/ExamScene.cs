using System.IO;
using Raytracer.Core.Materials;
using Raytracer.Core.Math;
using Raytracer.Core.Objects;
using Raytracer.IO;

namespace Raytracer.Core.Scenes;

public class ExamScene : BaseScene
{
    // Assets path
    private static readonly string AssetsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
    
    public override string Name => "Exam Scene";

    public override void Initialize(Scene scene)
    {
        Console.WriteLine("Initializing Exam Scene");
        
        SetupCamera(scene, new Vector3(0f, 10f, 0f), new Vector3(-1f, -1f, 1f));
        
        var light = new Light(new Color(1f, 1f, 1f));
        light.transform.position = new Vector3(-2f, 4f, -2f);
        scene.AddLight(light);
        
        SetupGroundAndWalls(scene);
        SetupBall(scene);
        RenderText(scene);
    }

    private void RenderText(Scene scene)
    {
        var text = ObjLoader.LoadFromFile(Path.Combine(AssetsDirectory, "ExamText.obj"));
        text.transform.position = new Vector3(-9f, 2.5f, 2.75f);
        text.transform.Scale = new Vector3(1.5f, 1.5f, 1.5f);
        text.transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitY, -(float)System.Math.PI / 2f);
        
        scene.AddObject(text);
    }

    private void SetupBall(Scene scene)
    {
    var ball = new Sphere(2.5f);
        ball.transform.position = new Vector3(-6f, 3f, 6f);
        ball.material = MaterialLibrary.Glass;
        scene.AddObject(ball);
    }

    private void SetupGroundAndWalls(Scene scene)
    {
        var ground = new Plane(20f, 20f);
        ground.transform.position = new Vector3(0f, 0f, 0f);
        ground.material = MaterialLibrary.CyanPlastic;
        scene.AddObject(ground);

        var leftWall = new Plane(5f, 20f);
        leftWall.transform.position = new Vector3(-10f, 2.5f, 0f);
        leftWall.transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, (float)System.Math.PI / 2f);
        leftWall.material = MaterialLibrary.RedPlastic;
        scene.AddObject(leftWall);
        
        var rightWall = new Plane(20f, 5f);
        rightWall.transform.position = new Vector3(0f, 2.5f, 10f);
        rightWall.transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitX, -(float)System.Math.PI / 2f);
        rightWall.material = MaterialLibrary.GreenPlastic;
        scene.AddObject(rightWall);
    }
}