using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject mobile;
    public Character character;
    public Vector3 goal;

    public float t = 0;

    private void Start()
    {
        if (mobile == null)
        {
            mobile = new GameObject();
            SpriteRenderer sr = mobile.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("Square");
            sr.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (character != null)
        {
            Translate(character.gameObject);
        }
    }

    public void ChangeTarget()
    {
        //goal = new Vector3(Random.Range(-100, 100), 0, 0);
    }

    public void Translate(GameObject mobileGO)
    {
        if (Vector3.Distance(mobileGO.transform.localPosition, goal) > 0.01f)
        {
            t += 0.1f * Time.deltaTime * (1 - Time.deltaTime);
            ChaseTarget(mobileGO, t);
        }

        else
        {
            ChangeTarget();
            t = 0;
        }
    }

    void ChaseTarget(GameObject mobileGO, float t)
    {
        Vector3 lastPosition = mobileGO.transform.localPosition;
        mobileGO.transform.localPosition = Vector2.Lerp(mobileGO.transform.localPosition, goal, t);

        Vector3 normal = mobileGO.transform.localPosition - lastPosition;

        Vector3 velocity = normal / Time.deltaTime;
        Vector3 tangent;
        if (velocity.x > 0) { tangent = new Vector3(-velocity.y, velocity.x); }
        else if (velocity.x < 0) { tangent = new Vector3(velocity.y, -velocity.x); }
        else if (velocity.y > 0) { tangent = velocity.magnitude * Vector3.up; }
        else if (velocity.y < 0) { tangent = velocity.magnitude * -Vector3.up; }
        else { tangent = Vector3.zero; }
        Vector3 accelerationTilt = tangent + velocity.magnitude * normal;
        mobileGO.transform.rotation = Quaternion.LookRotation(Vector3.forward, accelerationTilt);

    }
}
