namespace Raytracer.Core {

using System;

public struct Quaternion
{
    public float X { get; }
    public float Y { get; }
    public float Z { get; }
    public float W { get; }

    public static readonly Quaternion Identity = new Quaternion(0, 0, 0, 1);

    public Quaternion(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    // Length (magnitude) of the quaternion
    public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

    // Normalized quaternion
    public Quaternion Normalized
    {
        get
        {
            float mag = Magnitude;
            return mag > 0 ? new Quaternion(X / mag, Y / mag, Z / mag, W / mag) : Identity;
        }
    }

    // Conjugate of the quaternion
    public Quaternion Conjugate => new Quaternion(-X, -Y, -Z, W);

    // Inverse of the quaternion
    public Quaternion Inverse
    {
        get
        {
            float magSq = X * X + Y * Y + Z * Z + W * W;
            return magSq > 0 ? new Quaternion(-X / magSq, -Y / magSq, -Z / magSq, W / magSq) : Identity;
        }
    }

    // Multiply two quaternions
    public static Quaternion operator *(Quaternion q1, Quaternion q2)
    {
        return new Quaternion(
            q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y,
            q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X,
            q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W,
            q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z
        );
    }

    // Rotate a vector using this quaternion
    public static Vector3 operator *(Quaternion q, Vector3 v)
    {
        Quaternion vQuat = new Quaternion(v.X, v.Y, v.Z, 0);
        Quaternion result = q * vQuat * q.Inverse;
        return new Vector3(result.X, result.Y, result.Z);
    }

    // Create a quaternion from an axis and an angle (radians)
    public static Quaternion FromAxisAngle(Vector3 axis, float angle)
    {
        axis = axis.Normalized;
        float halfAngle = angle / 2;
        float sinHalf = (float)Math.Sin(halfAngle);
        return new Quaternion(axis.X * sinHalf, axis.Y * sinHalf, axis.Z * sinHalf, (float)Math.Cos(halfAngle));
    }

    // Convert quaternion to axis-angle representation
    public void ToAxisAngle(out Vector3 axis, out float angle)
    {
        if (Math.Abs(W) > 1)
            throw new Exception("Invalid Quaternion W value.");

        angle = 2 * (float)Math.Acos(W);
        float sinHalfAngle = (float)Math.Sqrt(1 - W * W);
        
        if (sinHalfAngle > 0.001f)
        {
            axis = new Vector3(X / sinHalfAngle, Y / sinHalfAngle, Z / sinHalfAngle);
        }
        else
        {
            axis = new Vector3(1, 0, 0); // Default axis
        }
    }

    // Convert Euler angles (radians) to quaternion
    public static Quaternion FromEulerAngles(float pitch, float yaw, float roll)
    {
        float cy = (float)Math.Cos(yaw * 0.5);
        float sy = (float)Math.Sin(yaw * 0.5);
        float cp = (float)Math.Cos(pitch * 0.5);
        float sp = (float)Math.Sin(pitch * 0.5);
        float cr = (float)Math.Cos(roll * 0.5);
        float sr = (float)Math.Sin(roll * 0.5);

        return new Quaternion(
            sr * cp * cy - cr * sp * sy,
            cr * sp * cy + sr * cp * sy,
            cr * cp * sy - sr * sp * cy,
            cr * cp * cy + sr * sp * sy
        );
    }

    // Convert quaternion to Euler angles (radians)
    public Vector3 ToEulerAngles()
    {
        float sinr_cosp = 2 * (W * X + Y * Z);
        float cosr_cosp = 1 - 2 * (X * X + Y * Y);
        float roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);

        float sinp = 2 * (W * Y - Z * X);
        float pitch = Math.Abs(sinp) >= 1 ? (float)Math.CopySign(Math.PI / 2, sinp) : (float)Math.Asin(sinp);

        float siny_cosp = 2 * (W * Z + X * Y);
        float cosy_cosp = 1 - 2 * (Y * Y + Z * Z);
        float yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);

        return new Vector3(pitch, yaw, roll);
    }

    public override string ToString()
    {
        return $"Quaternion({X}, {Y}, {Z}, {W})";
    }
}

}