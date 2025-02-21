namespace Raytracer.Core {

	public struct Quaternion
	{
    	public float W, X, Y, Z;

    	// Constructor
    	public Quaternion(float w, float x, float y, float z)
    	{
        	W = w;
        	X = x;
        	Y = y;
        	Z = z;
    	}

    	// Identity Quaternion
    	public static Quaternion Identity => new Quaternion(1, 0, 0, 0);

    	// Magnitude (Length)
    	public float Magnitude => (float)Math.Sqrt(W * W + X * X + Y * Y + Z * Z);

    	// Normalized Quaternion
    	public Quaternion Normalized
    	{
        	get
        	{
            	float mag = Magnitude;
            	if (mag == 0) throw new DivideByZeroException("Cannot normalize a zero-magnitude quaternion.");
            	return new Quaternion(W / mag, X / mag, Y / mag, Z / mag);
        	}
    	}

    	// Conjugate of the Quaternion
    	public Quaternion Conjugate()
    	{
        	return new Quaternion(W, -X, -Y, -Z);
    	}

    	// Inverse of the Quaternion
    	public Quaternion Inverse()
    	{
        	float magSq = W * W + X * X + Y * Y + Z * Z;
        	if (magSq == 0) throw new DivideByZeroException("Cannot invert a zero-magnitude quaternion.");
        	Quaternion conj = Conjugate();
        	return new Quaternion(conj.W / magSq, conj.X / magSq, conj.Y / magSq, conj.Z / magSq);
    	}

    	// Quaternion Multiplication (Combining Rotations)
    	public static Quaternion operator *(Quaternion q1, Quaternion q2)
    	{
        	return new Quaternion(
            	q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z,
            	q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y,
            	q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X,
            	q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W
        	);
    	}

    	// Rotating a Vector using a Quaternion
    	public static Vector3 Rotate(Vector3 v, Quaternion q)
    	{
        	Quaternion qv = new Quaternion(0, v.X, v.Y, v.Z);
        	Quaternion rotatedQ = q * qv * q.Inverse();
        	return new Vector3(rotatedQ.X, rotatedQ.Y, rotatedQ.Z);
    	}

    	// Create a Quaternion from an Axis-Angle Rotation
    	public static Quaternion FromAxisAngle(Vector3 axis, float angleDegrees)
    	{
        	float angleRadians = angleDegrees * (float)Math.PI / 180f;
        	float halfAngle = angleRadians / 2;
        	float sinHalf = (float)Math.Sin(halfAngle);

        	return new Quaternion(
            	(float)Math.Cos(halfAngle),
            	axis.X * sinHalf,
            	axis.Y * sinHalf,
            	axis.Z * sinHalf
        	).Normalized;
    	}

    	// Convert to Euler Angles (Yaw, Pitch, Roll)
    	public Vector3 ToEulerAngles()
    	{
        	float sinr_cosp = 2 * (W * X + Y * Z);
        	float cosr_cosp = 1 - 2 * (X * X + Y * Y);
        	float roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);

        	float sinp = 2 * (W * Y - Z * X);
        	float pitch = Math.Abs(sinp) >= 1 ? (float)Math.CopySign((float)Math.PI / 2, sinp) : (float)Math.Asin(sinp);

        	float siny_cosp = 2 * (W * Z + X * Y);
        	float cosy_cosp = 1 - 2 * (Y * Y + Z * Z);
        	float yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);

        	return new Vector3(roll, pitch, yaw) * (180f / (float)Math.PI);
    	}

    	// String representation for debugging
    	public override string ToString()
    	{
        	return $"({W}, {X}, {Y}, {Z})";
    	}
	}

}