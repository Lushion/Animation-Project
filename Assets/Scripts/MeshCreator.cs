using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    [HideInInspector] int ownerID; // one unique mesh

    // Mesh material
    public Material material;
    public MeshFilter mf;
    public MeshRenderer mr;

    // Lists for mesh creation
    protected List<Vector3> vertices = new List<Vector3>();
    protected List<Vector3> normals = new List<Vector3>();
    protected List<Vector2> uvs = new List<Vector2>();
    protected List<int> triangles = new List<int>();

    // List of Points
    public List<Vector3> meshPoints = new List<Vector3>();
    public int N { get { return meshPoints.Count; } }

    // Number of steps between two points
    protected int step = 1;

    public virtual void CreateMesh()
    {
        // Adding the mesh component
        if (mf == null)
        {
            mf = gameObject.AddComponent<MeshFilter>();
        }
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }
        Mesh mesh = mf.sharedMesh;

        vertices.Clear();
        normals.Clear();
        uvs.Clear();
        triangles.Clear();

        UpdateLists();

        // Creates the mesh
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    public virtual void UpdateLists() { }

    public virtual void CreateQuadMesh(Vector2 A, Vector2 B, Vector2 C, Vector2 D, int index)
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
        triangles.Add(index * 4 + 0);
        triangles.Add(index * 4 + 1);
        triangles.Add(index * 4 + 2);

        // Lower Triangle
        triangles.Add(index * 4 + 2);
        triangles.Add(index * 4 + 3);
        triangles.Add(index * 4 + 0);
    }
}
