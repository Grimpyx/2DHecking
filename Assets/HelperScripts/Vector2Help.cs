using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Help
{
    /// <summary>
    /// Rotates a vector clockwise by a degree amount.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="angleInDegrees"></param>
    /// <returns></returns>
    public static Vector2 RotateVector(Vector2 vector, float angleInDegrees)
    {
        angleInDegrees *= Mathf.Deg2Rad;

        float[,] rotMatrix = new float[2, 2] { { Mathf.Cos(angleInDegrees), Mathf.Sin(angleInDegrees) }, { -Mathf.Sin(angleInDegrees), Mathf.Cos(angleInDegrees) } };

        float newX = Vector2.Dot(new Vector2(rotMatrix[0, 0], rotMatrix[0, 1]), vector);
        float newY = Vector2.Dot(new Vector2(rotMatrix[1, 0], rotMatrix[1, 1]), vector);

        return new Vector2(newX, newY);
    }

    /// <summary>
    /// Projection of a onto b.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector2 ProjectVector(Vector2 a, Vector2 b)
    {
        float a_1 = Vector2.Dot(a, b) / b.magnitude;
        return a_1 * b.normalized;
    }

    /*public static Vector2 ClampLength(Vector2 v, float a, float b)
    {
        float l = v.magnitude;
        l = Mathf.Clamp(l, a, b);
        return Vector2
    }*/
}
