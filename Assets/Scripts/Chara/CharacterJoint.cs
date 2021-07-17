using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterJoint : MonoBehaviour
{
    
    public int id;

    public Image image;
    public Character character;
    public Handle handle;
    public Vertex point;
    public Vector3 localCoordinates;

    public void Update()
    {
        if (id <= 14)
        {
            transform.localPosition = point.position;
        }
    }

    public void SetJoint(Character character, int id, Vector2 coord)
    {
        this.id = id;
        this.character = character;

        // Creating a GameObject for each joint and places them
        localCoordinates = coord;
        point = new Vertex(coord);
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        transform.localPosition = coord;

        // A Joint is represented by a "Circle" Sprite
        image = gameObject.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Circle");
        image.enabled = false; // false: no sprite displayed, true: display sprite
    }
}
