using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Handle : MonoBehaviour
{
    public Vertex vertex;
    public SpriteRenderer sr;
    public bool isFixed;
    public bool isTied;


    // Start is called before the first frame update
    public void Setup(Vertex vertex)
    {
        this.vertex = vertex;
        transform.position = vertex.position;

        // A Joint is represented by a "Circle" Sprite
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Circle");
        sr.sortingOrder = 1;
        float size = (float)Camera.main.orthographicSize / 10f;
        transform.localScale = new Vector3(size, size, 0);

        
        gameObject.AddComponent<BoxCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<BoxCollider>().enabled = sr.enabled;

        // The Handle is fixed to the assigned vertex, cannot move unless vertex is moved.
        if (isFixed)
        {
            vertex.position = transform.position;
        }

        // The handle is tied to a vertex. When the handle moves, the vertex moves too.
        if (isTied)
        {
            transform.position = vertex.position;
        }
    }
}
