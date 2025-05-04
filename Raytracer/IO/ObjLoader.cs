using System.IO;
using Raytracer.Core.Math;
using Raytracer.Core.Objects;

namespace Raytracer.IO;

public static class ObjLoader
{
    public static Mesh LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"OBJ file not found: {filePath}");
        }

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var mesh = new Mesh(fileName);
        
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        
        // Add a dummy at index 0 because OBJ indices are 1-based
        vertices.Add(Vector3.Zero);
        normals.Add(Vector3.Zero);

        using (StreamReader reader = new StreamReader(filePath))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue; // Skip comments and empty lines
                }

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                string type = parts[0].ToLower();

                switch (type)
                {
                    case "v": // Vertex
                        if (parts.Length >= 4)
                        {
                            float x = float.Parse(parts[1]);
                            float y = float.Parse(parts[2]);
                            float z = float.Parse(parts[3]);
                            vertices.Add(new Vector3(x, y, z));
                        }
                        break;

                    case "vn": // Normal
                        if (parts.Length >= 4)
                        {
                            float x = float.Parse(parts[1]);
                            float y = float.Parse(parts[2]);
                            float z = float.Parse(parts[3]);
                            normals.Add(new Vector3(x, y, z));
                        }
                        break;

                    case "f": // Face
                        if (parts.Length >= 4) // Need at least 3 vertices for a face
                        {
                            ProcessFace(parts, vertices, normals, mesh);
                        }
                        break;
                }
            }
        }

        Console.WriteLine($"Loaded mesh '{mesh.Name}' with {vertices.Count-1} vertices and {mesh.GetTriangleCount()} triangles");
        return mesh;
    }
    
    private static void ProcessFace(string[] parts, List<Vector3> vertices, List<Vector3> normals, Mesh mesh)
    {
        // Handle triangles or quads (and potentially ngons by triangulating)
        var faceVertices = new List<Vector3>();
        var faceNormals = new List<Vector3>();

        // Parse each vertex description (which might include vertex/texture/normal indices)
        for (int i = 1; i < parts.Length; i++)
        {
            var indices = parts[i].Split('/');
            if (indices.Length > 0 && int.TryParse(indices[0], out int vertexIndex) && vertexIndex > 0)
            {
                // Add vertex
                faceVertices.Add(vertices[vertexIndex]);
                
                // Add normal if available
                if (indices.Length >= 3 && int.TryParse(indices[2], out int normalIndex) && normalIndex > 0)
                {
                    faceNormals.Add(normals[normalIndex]);
                }
                else
                {
                    // If normal is not specified, we'll calculate it later
                    faceNormals.Add(Vector3.Zero);
                }
            }
        }

        // Triangulate face (assume convex polygons)
        for (int i = 0; i < faceVertices.Count - 2; i++)
        {
            var v1 = faceVertices[0];
            var v2 = faceVertices[i + 1];
            var v3 = faceVertices[i + 2];
            
            // Create triangle
            var triangle = new Triangle(v1, v2, v3);
            mesh.AddTriangle(triangle);
        }
    }
}
