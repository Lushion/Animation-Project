using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationTilt : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    public List<Vector3> meshPoints;
    public List<Vector3> contourPoints;
    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> triangles = new List<int>();

    GameObject movementTarget;
    public Vector3 initialPosition;
    float timer;

    public float strength;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        timer = 0;

        movementTarget = new GameObject();
        movementTarget.name = "Mouvement Target";
        movementTarget.transform.position = Vector3.zero;

        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Square");
        sr.enabled = false;

        //TestMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, movementTarget.transform.position) > 0.01f)
        {
            timer += 0.01f * Time.deltaTime * (1 - Time.deltaTime);
            ChaseTarget(timer);
        }

        else
        {
            initialPosition = movementTarget.transform.position;
            timer = 0;
        }

    }

    void ChaseTarget(float t)
    {
        Vector3 lastPosition = transform.position;
        transform.position = Vector2.Lerp(transform.position, movementTarget.transform.position, t);

        Vector3 normal = transform.position - lastPosition;

        Vector3 velocity = normal / Time.deltaTime;
        Vector3 tangent;
        if (velocity.x > 0) { tangent = new Vector3(-velocity.y, velocity.x); }
        else if(velocity.x < 0) { tangent = new Vector3(velocity.y, -velocity.x); }
        else if (velocity.y > 0) { tangent = velocity.magnitude * Vector3.up; }
        else if (velocity.y < 0) { tangent = velocity.magnitude * -Vector3.up; }
        else { tangent = Vector3.zero; }
        Vector3 accelerationTilt = tangent + velocity.magnitude * normal / 10;

        //Debug.DrawLine(transform.localPosition, tangent - transform.localPosition);


        transform.rotation = Quaternion.LookRotation(Vector3.forward, accelerationTilt);
        //transform.rotation = Quaternion.FromToRotation(Vector3.up, accelerationTilt)
    }

    void TestMesh()
    {
        int NContourPoints = 10;

        GameObject meshTestGO = new GameObject();
        meshTestGO.transform.SetParent(gameObject.transform);
        meshTestGO.transform.localPosition = new Vector2(0, 0);
        MeshFilter mf = meshTestGO.AddComponent<MeshFilter>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }
        mesh = mf.sharedMesh;

        int index = 0;
        for (int i = 0; i < NContourPoints / 2; i++)
        {
            Vector2 A = new Vector2(i, 0);
            Vector2 B = new Vector2(i + 1, 0);
            Vector2 C = new Vector2(i, 1);
            Vector2 D = new Vector2(i + 1, 1);
            UpdateLists(A, B, C, D, index);
            contourPoints.Add(A);
            contourPoints.Add(C);
            index++;
        }


        // Creates the mesh
        EditMesh();

        // Renders the mesh
        MeshRenderer mr = meshTestGO.AddComponent<MeshRenderer>();
        mr.material = Resources.Load<Material>("Skin");
    }

    void EditMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
    }

    void UpdateLists(Vector2 A, Vector2 B, Vector2 C, Vector2 D, int index)
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
        triangles.Add(index * 4 + 1);
    }
}
