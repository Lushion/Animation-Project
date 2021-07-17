using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchSquashTriangle : MonoBehaviour
{
    // Vertices of triangle
    Triangle ABC;
    public Vertex A { get { return ABC.A; } }
    public Vertex B { get { return ABC.B; } }
    public Vertex C { get { return ABC.C; } }

    // Initial vertex positions of the triangle
    public Vector3 a0;
    public Vector3 b0;
    public Vector3 c0;
    float surfaceArea;

    // Projection of vertices on opposite edges
    Vector3 HC { get { return a0 + (c0 - a0).magnitude * CosineRuleA() *(b0 - a0).normalized; } }
    //Vector3 HC { get { return Angle.GetProjection(c0, a0, b0); } }

    //public void Start()
    //{
    //    Vertex A = new Vertex(a0);
    //    Vertex B = new Vertex(b0);
    //    Vertex C = new Vertex(c0);
    //    ABC = new Triangle(A, B, C);

    //    surfaceArea = Vector3.Distance(a0, b0) * Vector3.Distance(c0, HC) / 2;
    //}

    public void Update()
    {
        calcC();
        //Debug.DrawLine(A.position, B.position);
        //Debug.DrawLine(C.position, B.position);
        //Debug.DrawLine(A.position, C.position);
    }

    public void Setup(Vertex A, Vertex B, Vertex C)
    {
        // Updates initial positions
        this.a0 = A.position;
        this.b0 = B.position;
        this.c0 = C.position;
        ABC = new Triangle(A, B, C);

        
        surfaceArea = Vector3.Distance(a0, b0) * Vector3.Distance(c0, HC) / 2;
    }

    public Vector3 calcC()
    {
        // Calculates new height
        int angle = 1;
        if  (!Angle.CCW(new Vertex(c0), new Vertex(a0), new Vertex(b0))){ angle = -1; }
        float h = 2 * surfaceArea / (B.position - A.position).magnitude *angle;

        // Calculates new C point coordinates
        Vector3 tan = new Vector3(-(B.position - A.position).y, (B.position - A.position).x, 0).normalized;

        if (Vector3.Distance(b0, a0) != 0)
        {
            C.position = A.position + (HC - a0).x / (b0 - a0).x * (B.position - A.position).magnitude * (B.position - A.position).normalized + h * tan;
        }

        else
        {
            Debug.LogError("Spring has an initial length of 0");
        }

        return C.position;
    }


    public float CosineRuleA()
    {
        float b = Vector3.Distance(c0, a0);
        float c = Vector3.Distance(b0, a0);
        float a = Vector3.Distance(b0, c0);
        float cosA = ((b * b) + (c * c) - (a * a)) / (2 * b * c);

        if (!float.IsNaN(Mathf.Acos(cosA)))
        {
            return cosA;
        }
        else
        {
            return 1;
        }
    }

}
