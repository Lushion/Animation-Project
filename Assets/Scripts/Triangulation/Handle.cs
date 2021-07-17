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

        gameObject.AddComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFixed)
        {
            vertex.position = transform.position;
        }

        if (isTied)
        {
            transform.position = vertex.position;
        }
    }
}
