using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Angle
{

    // Finds the angle between two bases;
    static public float GetXYBasisRotationAngle(Vector3 newXaxis)
    {
        // Returns the right value
        if (newXaxis.x > 0) { return Mathf.Round(Mathf.Atan(newXaxis.y / newXaxis.x) * Mathf.Rad2Deg * 100) * 0.01f; }
        else if (newXaxis.x < 0) { return Mathf.Round((Mathf.Atan(newXaxis.y / newXaxis.x) * Mathf.Rad2Deg + 180)* 100) * 0.01f; }
        else if (newXaxis.y > 0) { return Mathf.PI / 2 * Mathf.Rad2Deg; }
        else { return -Mathf.PI / 2 * Mathf.Rad2Deg; }
    }
    static public float GetXZRotationBasisAngle(Vector3 newXaxis)
    {
        // Returns the right value

        if (newXaxis.x > 0) { return Mathf.Round(Mathf.Atan(newXaxis.z / newXaxis.x) * Mathf.Rad2Deg * 100) * 0.01f; }
        else if (newXaxis.x < 0) { return Mathf.Round((Mathf.Atan(newXaxis.z / newXaxis.x) * Mathf.Rad2Deg + 180) * 100) * 0.01f; }
        else if (newXaxis.z > 0) { return Mathf.PI / 2 * Mathf.Rad2Deg; }
        else { return -Mathf.PI / 2 * Mathf.Rad2Deg; }
    }

    static public float GetAngleProjection(Vector3 pointToProject, Vector3 originBasis, Vector3 projectionDirection)
    {
        // Calculate necessary parameters
        float alpha = GetXYBasisRotationAngle(projectionDirection);
        float x = Mathf.Cos(alpha) * (pointToProject - originBasis).x + Mathf.Sin(alpha) * (pointToProject - originBasis).y;
        float y = -Mathf.Sin(alpha) * (pointToProject - originBasis).x + Mathf.Cos(alpha) * (pointToProject - originBasis).y;

        // Returns the right value
        if (x > 0) { return Mathf.Round(Mathf.Atan(y / x) * Mathf.Rad2Deg * 100) * 0.01f; }
        else if (x < 0) { return Mathf.Round((Mathf.Atan(y / x) * Mathf.Rad2Deg + 180) * 100) * 0.01f; }
        else if (y > 0) { return Mathf.PI / 2; }
        else { return -Mathf.PI / 2; }
    }

    static public Vector3 GetProjection(Vector3 pointToProject, Vector3 pointOriginBasis, Vector3 lastPoint)
    {
        float alpha = GetXYBasisRotationAngle(lastPoint - pointOriginBasis);
        Vector3 pointChangedOrigin = pointToProject - pointOriginBasis;
        Vector3 newBasisPointToProject = new Vector3(pointChangedOrigin.x * Mathf.Cos(alpha) + pointChangedOrigin.y * Mathf.Sin(alpha), -pointChangedOrigin.x * Mathf.Sin(alpha) + pointChangedOrigin.y * Mathf.Cos(alpha), 0);

        return pointOriginBasis + newBasisPointToProject.x * (lastPoint - pointOriginBasis).normalized;
    }


    /// <summary>
    /// Checks the predicate CCW (A, B, C): "are A, B and C oriented in counter-clockwise?"
    /// </summary>
    /// <param name="A">Vertex A</param>
    /// <param name="B">Vertex B</param>
    /// <param name="C">Vertex C</param>
    /// <returns>True if A, B and C are oriented counter-clockwise, False otherwise</returns>
    public static bool CCW(Vertex A, Vertex B, Vertex C)
    {
        if (A.position.x * (B.position.y - C.position.y) - B.position.x * (A.position.y - C.position.y) + C.position.x * (A.position.y - B.position.y) > 0) { return true; }
        return false;
    }
}
