using System;

namespace Raytracer.Core.Math
{
    /// <summary>
    /// Represents an Axis-Aligned Bounding Box for efficient collision detection
    /// </summary>
    public class AABB
    {
        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }

        // Constructor for empty AABB (invalid min/max)
        public AABB()
        {
            Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        }

        // Constructor with min and max points
        public AABB(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        // Check if this AABB is valid (has a non-zero volume)
        public bool IsValid()
        {
            return Min.X <= Max.X && Min.Y <= Max.Y && Min.Z <= Max.Z;
        }

        // Expand AABB to include a point
        public void ExpandToInclude(Vector3 point)
        {
            Min = new Vector3(
                System.Math.Min(Min.X, point.X),
                System.Math.Min(Min.Y, point.Y),
                System.Math.Min(Min.Z, point.Z)
            );

            Max = new Vector3(
                System.Math.Max(Max.X, point.X),
                System.Math.Max(Max.Y, point.Y),
                System.Math.Max(Max.Z, point.Z)
            );
        }

        // Expand AABB to include another AABB
        public void ExpandToInclude(AABB other)
        {
            if (!other.IsValid())
                return;

            Min = new Vector3(
                System.Math.Min(Min.X, other.Min.X),
                System.Math.Min(Min.Y, other.Min.Y),
                System.Math.Min(Min.Z, other.Min.Z)
            );

            Max = new Vector3(
                System.Math.Max(Max.X, other.Max.X),
                System.Math.Max(Max.Y, other.Max.Y),
                System.Math.Max(Max.Z, other.Max.Z)
            );
        }

        // Check intersection with a ray, returns t-values where ray enters/exits box
        public bool IntersectRay(Ray ray, out float tMin, out float tMax)
        {
            tMin = float.MinValue;
            tMax = float.MaxValue;

            // For each axis (X, Y, Z)
            for (int i = 0; i < 3; i++)
            {
                float minComponent = 0, maxComponent = 0, originComponent = 0, dirComponent = 0;

                // Extract the current axis components
                switch (i)
                {
                    case 0: // X axis
                        minComponent = Min.X;
                        maxComponent = Max.X;
                        originComponent = ray.Origin.X;
                        dirComponent = ray.Direction.X;
                        break;
                    case 1: // Y axis
                        minComponent = Min.Y;
                        maxComponent = Max.Y;
                        originComponent = ray.Origin.Y;
                        dirComponent = ray.Direction.Y;
                        break;
                    case 2: // Z axis
                        minComponent = Min.Z;
                        maxComponent = Max.Z;
                        originComponent = ray.Origin.Z;
                        dirComponent = ray.Direction.Z;
                        break;
                }

                // Handle division by zero (ray parallel to plane)
                if (System.Math.Abs(dirComponent) < float.Epsilon)
                {
                    // If origin is outside the slab, no intersection
                    if (originComponent < minComponent || originComponent > maxComponent)
                        return false;
                }
                else
                {
                    // Compute the intersection t values with the near and far planes
                    float t1 = (minComponent - originComponent) / dirComponent;
                    float t2 = (maxComponent - originComponent) / dirComponent;

                    // Ensure t1 is the nearest intersection
                    if (t1 > t2)
                    {
                        float temp = t1;
                        t1 = t2;
                        t2 = temp;
                    }

                    // Update min/max intersection
                    tMin = System.Math.Max(tMin, t1);
                    tMax = System.Math.Min(tMax, t2);

                    // Early exit - if we don't hit this slab, we don't hit the box
                    if (tMin > tMax)
                        return false;
                }
            }

            // We've survived all three axis tests
            return true;
        }

        // Transform this AABB by the given transform
        public AABB Transform(Transform transform)
        {
            // Get the 8 corners of the current bounding box
            Vector3[] corners = new Vector3[8]
            {
                new Vector3(Min.X, Min.Y, Min.Z),
                new Vector3(Min.X, Min.Y, Max.Z),
                new Vector3(Min.X, Max.Y, Min.Z),
                new Vector3(Min.X, Max.Y, Max.Z),
                new Vector3(Max.X, Min.Y, Min.Z),
                new Vector3(Max.X, Min.Y, Max.Z),
                new Vector3(Max.X, Max.Y, Min.Z),
                new Vector3(Max.X, Max.Y, Max.Z)
            };

            // Transform each corner and find the new bounding box
            AABB result = new AABB();
            
            for (int i = 0; i < 8; i++)
            {
                Vector3 transformedCorner = transform.TransformToWorldPosition(corners[i]);
                result.ExpandToInclude(transformedCorner);
            }

            return result;
        }
    }
}
