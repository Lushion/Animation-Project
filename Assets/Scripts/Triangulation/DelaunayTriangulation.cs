using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DelaunayTriangulation : MonoBehaviour
{
    public Dictionary<List<Vertex>, Triangle> triangles;
    public Dictionary<List<Vertex>, Edge> edges;
    public List<Vertex> vertices;

    public List<Vector3> vertexPositions;
    public List<List<Vector3>> edgeConstraints;


    //public List<Edge> Divide(List<Vertex> points)
    //{
    //    // Lines
    //    if (points.Count == 2)
    //    {
    //        Edge edge = new Edge(points[0], points[1]);
    //        return new List<Edge>() { new Edge(points[0], points[1]), new Edge(points[1], points[0]) };
    //    }

    //    // Triangles
    //    else if (points.Count == 3)
    //    {
    //        Edge segment1 = new Edge(points[0], points[1]);
    //        Edge segment2 = new Edge(points[1], points[2]);

    //        if (CCW(points[0], points[1], points[2]))
    //        {

    //        }
    //        Edge segment3 = new Edge(points[2], points[0]);
    //    }

    //    else
    //    {
    //        Divide(points.GetRange(0, points.Count / 2));
    //        Divide(points.GetRange(points.Count / 2, points.Count - (points.Count / 2)));
    //    }
    //}


    Triangle GetTriangle(Vertex A, Vertex B, Vertex C)
    {
        if (triangles[new List<Vertex>() { A, B, C }] != null)
        {
            return triangles[new List<Vertex>() { A, B, C }];
        }

        Debug.LogError("This triangle doesn't exist");
        return null;
    }


    void AddTriangle(Vertex A, Vertex B, Vertex C)
    {
        triangles.Add(new List<Vertex>() { A, B, C }, new Triangle(A, B, C));
    }

    void DeleteTriangle(Vertex A, Vertex B, Vertex C)
    {
        triangles.Remove(new List<Vertex>() { A, B, C });
    }

    Vertex Adjacent(Vertex A, Vertex B)
    {
        foreach (Triangle triangle in triangles.Values)
        {
            if (triangle.A == A && triangle.B == B && CCW(A, B, triangle.C))
            {
                return triangle.C;
            }
        }

        Debug.Log("No point found");
        return null;
    }



    /// <summary>
    /// Checks the predicate CCW (A, B, C): are A, B and C oriented in counter-clockwise?
    /// </summary>
    /// <param name="A">Vertex A</param>
    /// <param name="B">Vertex B</param>
    /// <param name="C">Vertex C</param>
    /// <returns>True if A, B and C are oriented counter-clockwise, False otherwise</returns>
    static bool CCW(Vertex A, Vertex B, Vertex C)
    {
        if (A.position.x * (B.position.y - C.position.y) + B.position.x * (A.position.y - C.position.y) + C.position.x * (A.position.y - B.position.y) > 0) { return true; }
        return false;
    }

    /// <summary>
    /// Checks the predicate InCircle (A, B, C, D): is D inside the circumcenter of the Triangle A, B, C
    /// </summary>
    /// <param name="A">Vertex A</param>
    /// <param name="B">Vertex B</param>
    /// <param name="C">Vertex C</param>
    /// <param name="D">Vertex D (to be checked)</param>
    /// <returns>True if D is inside the triangle ABD, False otherwise</returns>
    static bool inCircle(Vertex A, Vertex B, Vertex C, Vertex D)
    {
        List<List<float>> matrix = new List<List<float>>() { new List<float>(){ A.position.x - D.position.x, A.position.y - D.position.y, (A.position.x - D.position.x) * (A.position.x - D.position.x) + (A.position.y - D.position.y) * (A.position.y - D.position.y) },
                                                             new List<float>(){ B.position.x - D.position.x, B.position.y - D.position.y, (B.position.x - D.position.x) * (B.position.x - D.position.x) + (B.position.y - D.position.y) * (B.position.y - D.position.y) },
                                                             new List<float>(){ C.position.x - D.position.x, C.position.y - D.position.y, (C.position.x - D.position.x) * (C.position.x - D.position.x) + (C.position.y - D.position.y) * (C.position.y - D.position.y) } };

        float det = matrix[0][0] * (matrix[1][1] * matrix[2][2] - matrix[2][1] * matrix[1][2])
                  + matrix[1][0] * (matrix[0][1] * matrix[2][2] - matrix[2][1] * matrix[0][2])
                  + matrix[2][0] * (matrix[0][1] * matrix[1][2] - matrix[1][1] * matrix[0][2]);

        if (det > 0) { return true; }
        return false;
    }

    /// <summary>
    /// Sorts the list of vertices by x-coordinates. In case of ties, sorts by Y-coordinate. 
    /// </summary>
    public void SortVertices()
    {
        vertices.OrderBy(point => point.position.x).ThenBy(point => point.position.y).ToList();
    }


}
