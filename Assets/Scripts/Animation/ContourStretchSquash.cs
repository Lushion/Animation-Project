using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContourStretchSquash : MeshCreator
{
    // For testing purposes
    Vertex A;
    Vertex B;
    public Vector3 a;
    public Vector3 b;


    // Spring properties
    public List<StretchSquashTriangle> sts1 = new List<StretchSquashTriangle>();
    public List<StretchSquashTriangle> sts2 = new List<StretchSquashTriangle>();
    public List<Spring> springs = new List<Spring>();
    public Spring spring;
    public bool forceActive = true;

    // Mesh Contour properties
    public float thickness = 0.2f;
    public List<Vector3> points = new List<Vector3>();
    public List<Vertex> SplinePoints = new List<Vertex>();


    //Testing purposes
    public virtual void Start()
    {
        A = new Vertex(a);
        B = new Vertex(b);

        // Create a Spring and a Force
        spring = gameObject.AddComponent<Spring>();
        spring.Setup(A, B);

        foreach (Vector3 pos in points)
        {
            Vertex vertex = new Vertex(pos);
            SplinePoints.Add(vertex);

            StretchSquashTriangle st = gameObject.AddComponent<StretchSquashTriangle>();
            st.Setup(A, B, vertex);
            sts1.Add(st);
        }

        // Renders the mesh
        mr = gameObject.AddComponent<MeshRenderer>();
        material = Resources.Load<Material>("Line");
        mr.material = material;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        for (int i = 0; i < SplinePoints.Count; i++)
        {
            foreach (Spring spring in springs)
            {
                if (forceActive) { spring.forceActive = true; }
                else { spring.forceActive = false; }
            }
            SplinePoints[i].position = sts1[i].C.position;
        }

        if (SplinePoints.Count > 0)
        {
            DrawSpline();
        }
    }


    public void Setup(Vertex A, Vertex B)
    {
        // Create a Spring and a Force
        spring = gameObject.AddComponent<Spring>();
        spring.Setup(A, B);
        springs.Add(spring);

        foreach (Vector3 pos in points)
        {
            Vertex vertex = new Vertex(pos);
            SplinePoints.Add(vertex);

            StretchSquashTriangle st = gameObject.AddComponent<StretchSquashTriangle>();
            st.Setup(A, B, vertex);
            sts1.Add(st);
        }

        // Renders the mesh
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        material = Resources.Load<Material>("Line");
        mr.material = material;
    }

    // Draw the Spline
    public void DrawSpline()
    {
        // Shows Points on screen
        meshPoints.Clear();

        // Calculates the contour Catmull Rom spline
        for (int i = 0; i <= SplinePoints.Count; i++)
        {
            Vector2 P0 = SplinePoints[CalcLoopPoint(i - 1)].position;
            Vector2 P1 = SplinePoints[CalcLoopPoint(i)].position;
            Vector2 P2 = SplinePoints[CalcLoopPoint(i + 1)].position;
            Vector2 P3 = SplinePoints[CalcLoopPoint(i + 2)].position;
            Vector2[] P = new Vector2[] { P0, P1, P2, P3 };
            CatmullRomSpline(P);
        }

        CreateMesh();
    }

    // Updates the Vertices, Normals, UVs and Triangles lists at every change of index
    public override void UpdateLists()
    {
        for (int i = 0; i < ((meshPoints.Count / 2) - 1); i++)
        {
            int index = i * 2;
            CreateQuadMesh(meshPoints[index], meshPoints[index + 1], meshPoints[index + 2], meshPoints[index + 3], index);
        }
    }

    public override void CreateQuadMesh(Vector2 A, Vector2 B, Vector2 C, Vector2 D, int index)
    { 
        // Vertex array
        vertices.Add(A);
        vertices.Add(B);
        vertices.Add(C);
        vertices.Add(D);

        // Normal array
        normals.Add(new Vector3(0, 0, 1));
        normals.Add(new Vector3(0, 0, 1));
        normals.Add(new Vector3(0, 0, 1));
        normals.Add(new Vector3(0, 0, 1));

        // Texture coordinates
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));

        // Upper triangle
        triangles.Add(index * 2 + 0);
        triangles.Add(index * 2 + 1);
        triangles.Add(index * 2 + 2);

        // Lower Triangle
        triangles.Add(index * 2 + 2);
        triangles.Add(index * 2 + 3);
        triangles.Add(index * 2 + 1);
    }

    public int CalcLoopPoint(int i)
    {
        if (i < 0) { return i + SplinePoints.Count; }

        else if (i > SplinePoints.Count - 1) { return i - SplinePoints.Count; }

        else { return i; }
    }

    public void CatmullRomSpline(Vector2[] P)
    {
        // Resolution 
        float numberOfSteps = 10;

        // Start and end points of each calculated segment
        Vector2 startPoint = P[1];
        Vector2 endPoint;

        // Coefficients for Catmull Rom Spline
        Vector3 a = -P[0] + 3 * P[1] - 3 * P[2] + P[3];
        Vector3 b = 2 * P[0] - 5 * P[1] + 4 * P[2] - P[3];
        Vector3 c = -P[0] + P[2];
        Vector3 d = 2 * P[1];

        // Calculates the tangent to the contour curve
        Vector2 normal = (P[2] - P[0]).normalized;
        Vector2 tangent = new Vector2(-normal.y, normal.x) * thickness / 2;

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
    }

}
