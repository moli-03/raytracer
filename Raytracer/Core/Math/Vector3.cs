namespace Raytracer.Core.Objects
{
    public struct Vector3
    {
        
        public static readonly Vector3 Zero = new Vector3(0, 0, 0);
        public static readonly Vector3 UnitX = new Vector3(1, 0, 0);
        public static readonly Vector3 UnitY = new Vector3(0, 1, 0);
        public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);
        
        
        public float X, Y, Z;

        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float Magnitude
        {
            get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        public Vector3 Normalized
        {
            get
            {
                float length = Magnitude;
                if (length > 0)
                {
                    return this / length;
                }
                return Zero;
            }
        }

        public float SqrMagnitude
        {
            get { return X * X + Y * Y + Z * Z; }
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        
        public static Vector3 operator -(Vector3 v1)
        {
            return -1 * v1;
        }

        public static Vector3 operator *(Vector3 v, float scalar)
        {
            return new Vector3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static Vector3 operator *(float scalar, Vector3 v)
        {
            return new Vector3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static Vector3 operator /(Vector3 v, float scalar)
        {
            if (scalar != 0)
            {
                return new Vector3(v.X / scalar, v.Y / scalar, v.Z / scalar);
            }
            throw new DivideByZeroException("Scalar cannot be zero.");
        }

        public static float Dot(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X
            );
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3 other)
            {
                return X == other.X && Y == other.Y && Z == other.Z;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }

}
