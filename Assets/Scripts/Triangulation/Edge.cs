using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public int id;

    public Vertex origin; 
    public Vertex destination; 

    public Triangle leftFace;
    public Triangle rightFace;

    public float thickness = 2;

    public Edge(Vertex Origin, Vertex Dest)
    {
        this.origin = Origin;
        this.destination = Dest;
    }

    ///// <summary>
    ///// Flips the edge (same direction, different orientation). The triangles are upside down
    ///// </summary>
    ///// <returns></returns>
    //public Edge Flip()
    //{
    //    Edge flippedEdge = new Edge(origin, destination);
    //    //flippedEdge.leftFace = ;
    //    //flippedEdge.rightFace = ;
    //    return flippedEdge;
    //}

    /// <summary>
    /// Symmetry of the edge, edge from destination to origin
    /// </summary>
    /// <returns></returns>
    public Edge Sym()
    {
        Edge symEdge = new Edge(destination, origin);
        symEdge.leftFace = rightFace;
        symEdge.rightFace = leftFace;

        return symEdge;
    }

    /// <summary>
    /// Next Edge (CCW) with same left face with its origin = this edge's destination
    /// </summary>
    /// <returns></returns>
    public Edge LNext()
    {
        foreach (Edge edge in leftFace.edges.Values)
        {
            if (edge != this && edge.leftFace == leftFace && edge.origin == this.destination)
            {
                return edge;
            }
        }

        return null;
    }


    /// <summary>
    /// Next Edge (CCW) with same right face with its destination = this edge's origin
    /// </summary>
    /// <returns></returns>
    public Edge RNext()
    {
        foreach (Edge edge in rightFace.edges.Values)
        {
            if (edge != this && edge.rightFace == rightFace && edge.destination == this.origin)
            {
                return edge;
            }
        }

        return null;
    }

    /// <summary>
    /// Next edge (CCW) with same origin: edge in the left triangle which has its right triangle = this left triangle.
    /// </summary>
    /// <returns></returns>
    public Edge ONext()
    {
        foreach (Edge edge in leftFace.edges.Values)
        {
            if (edge != this && edge.origin == this.origin && edge.rightFace == this.leftFace)
            {
                return edge;
            }
        }

        return null;
    }


    /// <summary>
    /// Next edge (CCW) with same destination: edge in the right triangle which has its left triangle = this right triangle.
    /// </summary>
    /// <returns></returns>
    public Edge DNext()
    {
        foreach (Edge edge in rightFace.edges.Values)
        {
            if (edge != this && edge.destination == this.destination && edge.leftFace != this.rightFace)
            {
                return edge;
            }
        }

        return null;
    }



    /// <summary>
    /// Previous Edge (CW) with same left face with its destination = this edge's origin
    /// </summary>
    /// <returns></returns>
    public Edge LPrev()
    {
        foreach (Edge edge in leftFace.edges.Values)
        {
            if (edge != this && edge.leftFace == leftFace && edge.destination == this.origin)
            {
                return edge;
            }
        }

        return null;
    }

    /// <summary>
    /// Previous Edge (CW) with same right face with its origin = this edge's destination
    /// </summary>
    /// <returns></returns>
    public Edge RPrev()
    {
        foreach (Edge edge in rightFace.edges.Values)
        {
            if (edge != this && edge.rightFace == rightFace && edge.origin == this.destination)
            {
                return edge;
            }
        }

        return null;
    }

    /// <summary>
    /// Previous edge (CCW) with same origin: edge in the left triangle which has its left triangle = this right triangle.
    /// </summary>
    /// <returns></returns>
    public Edge OPrev()
    {
        foreach (Edge edge in leftFace.edges.Values)
        {
            if (edge != this && edge.origin == this.origin && edge.leftFace == rightFace)
            {
                return edge;
            }
        }

        return null;
    }

    /// <summary>
    /// Previous edge (CCW) with same destination: edge in the left triangle which has its right triangle = this left triangle.
    /// </summary>
    /// <returns></returns>
    public Edge DPrev()
    {
        foreach (Edge edge in leftFace.edges.Values)
        {
            if (edge != this && edge.destination == this.destination && edge.rightFace == leftFace)
            {
                return edge;
            }
        }

        return null;
    }

    // Draw the Spline
    /* CP = Control Point */
    public List<Vector3> CatmullRomSpline(Vector3 leftCP, Vector3 rightCP)
    {
        // Simplify notation
        Vector3 P0 = leftCP;
        Vector3 P1 = origin.position;
        Vector3 P2 = destination.position;
        Vector3 P3 = rightCP;

        // Start and end points of each calculated sub-segment
        Vector2 startPoint = P1;
        Vector2 endPoint;
        List<Vector3> meshPoints = new List<Vector3>();

        // Calculates the tangent to the contour curve
        Vector2 normal = (P2 - P0).normalized;
        Vector2 tangent = new Vector2(-normal.y, normal.x) * thickness / 2;

        // Coefficients for Catmull Rom Spline
        Vector3 a = -P0 + 3 * P1 - 3 * P2 + P3;
        Vector3 b = 2 * P0 - 5 * P1 + 4 * P2 - P3;
        Vector3 c = -P0 + P2;
        Vector3 d = 2 * P1;

        
        // Resolution 
        float numberOfSteps = 5;
        for (int step = 1; step < numberOfSteps; step++)
        {
            // Add 2 vertices to draw the contour line to the Points list
            Vector2 A = startPoint + tangent;
            Vector2 B = startPoint - tangent;
            meshPoints.Add(A);
            meshPoints.Add(B);

            // Calculate Catmull Rom Spline
            float t = step / numberOfSteps;
            endPoint = 0.5f * (a * t * t * t + b * t * t + c * t + d);

            normal = (endPoint - startPoint).normalized;
            tangent = new Vector2(-normal.y, normal.x) * thickness / 2;

            startPoint = endPoint;
        }

        return meshPoints;
    }
}
