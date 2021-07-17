using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public int id;
    public Vector3 position;
    public Dictionary<Vertex, Edge> edges;
    public List<Triangle> faces;

    public Vertex(Vector3 position)
    {
        this.position = position;
    }
}
