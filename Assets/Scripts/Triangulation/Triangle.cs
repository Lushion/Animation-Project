using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public int id;

    public List<Vertex> vertices;
    public Vertex A { get { return vertices[0]; } }
    public Vertex B { get { return vertices[1]; } }
    public Vertex C { get { return vertices[2]; } }

    public Dictionary<List<Vertex>, Edge> edges;


    // Edge Lengths
    public float LengthA { get { return Vector3.Distance(B.position, C.position); } }
    public float LengthB { get { return Vector3.Distance(A.position, C.position); } }
    public float LengthC { get { return Vector3.Distance(A.position, B.position); } }


    // Middle of a segment
    public Vector3 MAC { get { return (A.position + C.position) / 2; } } 
    public Vector3 MAB { get { return (A.position + B.position) / 2; } } 
    public Vector3 MBC { get { return (B.position + C.position) / 2; } } 


    // Projections
    public Vector3 HA { get { return Angle.GetProjection(A.position, B.position, C.position); } } // Returns vector: B -> HA
    public Vector3 HB { get { return Angle.GetProjection(B.position, A.position, C.position); } } // Returns vector: A -> HB
    public Vector3 HC { get { return Angle.GetProjection(C.position, A.position, B.position); } } // Returns vector: A -> HC


    // Physical parameters
    float SurfaceArea { get { return BaseLength * HeightLength; } }
    public float BaseLength { get { return Vector3.Distance(A.position, B.position); } }
    public float HeightLength { get { return Vector3.Distance(C.position, HC); } }

    public Triangle(Vertex A, Vertex B, Vertex C)
    {
        vertices = new List<Vertex>();
        vertices.Add(A);
        vertices.Add(B);
        vertices.Add(C);
    }
    
    /// <Summary>
    /// The circumcenter of the circumcircle of the triangle.
    /// </Summary>
    public Vector3 Circumcenter 
    {
        get
        {
            // AB: 1st line, AC: 2nd line
            Vector3 TAB = MAB + new Vector3(-(B.position - A.position).y, (B.position - A.position).x);
            Vector3 TAC = MAC + new Vector3(-(C.position - A.position).y, (C.position - A.position).x);

            float cross = (TAC - MAC).x * (TAB - MAB).y - (TAC - MAC).y * (TAB - MAB).x;

            if (cross == 0) { Debug.Log("No Solution for circumcenter"); return Vector3.zero; }

            else
            {
                float mu = ((MAB - MAC).x * (TAB - MAB).y - (MAB - MAC).y * (TAB - MAB).x) / cross;

                Vector3 circumcenter = new Vector3(
                    MAC.x + (TAC.x - MAC.x) * mu,
                    MAC.y + (TAC.y - MAC.y) * mu
                );

                return circumcenter;
            }
        }
    }

    public float CircumcircleRadius
    {
        get
        {
            return Vector3.Distance(A.position, Circumcenter);
        }
    }

    /// <summary>
    /// The next edge (i.e. going counter-clockwise)
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public Edge NextEdge(Edge edge)
    {
        foreach (Vertex vertex in vertices)
        {
            if (vertex != edge.origin && vertex != edge.destination)
            {
                return edges[new List<Vertex>() { edge.destination, vertex }];
            }
        }

        return null;
    }


    /// <summary>
    /// The previous edge (i.e. going clockwise)
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public Edge PrevEdge(Edge edge)
    {
        foreach (Vertex vertex in vertices)
        {
            if (vertex != edge.origin && vertex != edge.destination)
            {
                return edges[new List<Vertex>() { vertex, edge.origin }];
            }
        }

        return null;
    }

    /// <summary>
    /// The next edge (i.e. going counter-clockwise)
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public Edge ONextEdge(Edge edge)
    {
        foreach (Vertex vertex in vertices)
        {
            if (vertex != edge.origin && vertex != edge.destination)
            {
                return edges[new List<Vertex>() { edge.origin, vertex }];
            }
        }

        return null;
    }

    /// <summary>
    /// The next edge (i.e. going counter-clockwise)
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public Edge DNextEdge(Edge edge)
    {
        foreach (Vertex vertex in vertices)
        {
            if (vertex != edge.origin && vertex != edge.destination)
            {
                return edges[new List<Vertex>() { vertex, edge.destination }];
            }
        }

        return null;
    }




    // Calculates the angle A from the triangle ABC
    public float CosineRuleA()
    {
        float cosA = ((LengthB * LengthB) + (LengthC * LengthC) - (LengthA * LengthA)) / (2 * LengthB * LengthC);

        if (!float.IsNaN(Mathf.Acos(cosA)))
        {
            float rotationBasisAngle = Angle.GetXYBasisRotationAngle(B.position - C.position);
            if (-(A.position - B.position).x * Mathf.Sin(rotationBasisAngle) + (A.position - B.position).y * Mathf.Cos(rotationBasisAngle) >= 0)
            {
                return Mathf.Round(Mathf.Acos(cosA) * Mathf.Rad2Deg * 10) * 0.1f;
            }
            else
            {

                return Mathf.Round(-Mathf.Acos(cosA) * Mathf.Rad2Deg * 10) * 0.1f;
            }
        }
        else
        {
            return 1;
        }
    }

    // Calculates the angle B from the triangle ABC
    public float CosineRuleB()
    {
        float cosB = ((LengthA * LengthA) + (LengthC * LengthC) - (LengthB * LengthB)) / (2 * LengthA * LengthC);
        //Debug.Log(cosB);
        //Debug.Log(Mathf.Round(Mathf.Acos(cosB) * Mathf.Rad2Deg * 10) * 0.1f);

        if (!float.IsNaN(Mathf.Acos(cosB)))
        {
            float rotationBasisAngle = Angle.GetXYBasisRotationAngle(A.position - C.position);


            if (-(B.position - A.position).x * Mathf.Sin(rotationBasisAngle) + (B.position - A.position).y * Mathf.Cos(rotationBasisAngle) >= 0)
            {
                return Mathf.Round(Mathf.Acos(cosB) * Mathf.Rad2Deg * 10) * 0.1f;
            }
            else
            {
                return Mathf.Round(-Mathf.Acos(cosB) * Mathf.Rad2Deg * 10) * 0.1f;
            }
        }
        else
        {
            return 1;
        }
    }

    // Calculates the angle B from the triangle ABC
    public float CosineRuleC()
    {
        float cosC = ((LengthA * LengthA) + (LengthB * LengthB) - (LengthC * LengthC)) / (2 * LengthA * LengthB);

        if (!float.IsNaN(Mathf.Acos(cosC)))
        {
            float rotationBasisAngle = Angle.GetXYBasisRotationAngle(A.position - B.position);
            if (-(C.position - A.position).x * Mathf.Sin(rotationBasisAngle) + (C.position - A.position).y * Mathf.Cos(rotationBasisAngle) >= 0)
            {
                return Mathf.Round(Mathf.Acos(cosC) * Mathf.Rad2Deg * 10) * 0.1f;
            }
            else
            {

                return Mathf.Round(-Mathf.Acos(cosC) * Mathf.Rad2Deg * 10) * 0.1f;
            }
        }
        else
        {
            return 1;
        }
    }
}
