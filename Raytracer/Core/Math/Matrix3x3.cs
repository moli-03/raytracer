namespace Raytracer.Core.Objects;

public struct Matrix3x3
{
    private float[,] matrix;

    public Matrix3x3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
    {
        this.matrix = new float[,]
        {
            { m11, m12, m13 },
            { m21, m22, m23 },
            { m31, m32, m33 }
        };
    }

    public static Vector3 operator *(Matrix3x3 m, Vector3 v)
    {
        return new Vector3(
            m.matrix[0, 0] * v.X + m.matrix[0, 1] * v.Y + m.matrix[0, 2] * v.Z,
            m.matrix[1, 0] * v.X + m.matrix[1, 1] * v.Y + m.matrix[1, 2] * v.Z,
            m.matrix[2, 0] * v.X + m.matrix[2, 1] * v.Y + m.matrix[2, 2] * v.Z
        );
    }

    public float Det()
    {
        return matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1]) -
               matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0]) +
               matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);
    }

    public bool TryGetInverse(out Matrix3x3 inverse)
    {
        float det = Det();

        if (Math.Abs(det) < float.Epsilon)
        {
            inverse = default;
            return false;
        }

        float invDet = 1.0f / det;

        inverse = new Matrix3x3(
            (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1]) * invDet,
            (matrix[0, 2] * matrix[2, 1] - matrix[0, 1] * matrix[2, 2]) * invDet,
            (matrix[0, 1] * matrix[1, 2] - matrix[0, 2] * matrix[1, 1]) * invDet,
            (matrix[1, 2] * matrix[2, 0] - matrix[1, 0] * matrix[2, 2]) * invDet,
            (matrix[0, 0] * matrix[2, 2] - matrix[0, 2] * matrix[2, 0]) * invDet,
            (matrix[0, 2] * matrix[1, 0] - matrix[0, 0] * matrix[1, 2]) * invDet,
            (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]) * invDet,
            (matrix[0, 1] * matrix[2, 0] - matrix[0, 0] * matrix[2, 1]) * invDet,
            (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]) * invDet
        );

        return true;
    }
}
