using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringChain : MonoBehaviour
{
    public int NJoints = 3;
    public List<Spring> springs = new List<Spring>();
    public List<Vertex> joints = new List<Vertex>();
    public bool forceActive = true;

    // Testing Purposes
    //private void Start()
    //{
    //    for(int i = 0; i < NJoints; i++)
    //    {
    //        joints.Add(new Vertex(new Vector3(2 * i, 0, 0)));
    //    }

    //    for (int i = 0; i < NJoints - 1; i++)
    //    {
    //        Spring s = gameObject.AddComponent<Spring>();
    //        s.Setup(joints[i], joints[i+1]);
    //        if (i > 0)
    //        {
    //            s.handleA.isFixed = false;
    //            s.handleA.isTied = true;
    //            s.handleA.sr.enabled = false;

    //        }
    //        springs.Add(s);
    //    }
    //}

    public void Setup(List<Vertex> joints)
    {
        this.joints = joints;
    

        for (int i = 0; i < NJoints - 1; i++)
        {
            Spring s = gameObject.AddComponent<Spring>();
            s.Setup(joints[i], joints[i + 1]);
            if (i > 0)
            {
                s.handleA.isFixed = false;
                s.handleA.isTied = true;
                s.handleA.sr.enabled = false;
            }

            springs.Add(s);
        }
    }

    public void Setup(List<Spring> springs)
    {
        this.springs = springs;

        for (int i = 0; i < springs.Count; i++)
        {
            if (i > 0)
            {
                springs[i].handleA.isFixed = false;
                springs[i].handleA.isTied = true;
                springs[i].handleA.sr.enabled = false;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < springs.Count; i++)
        {
            springs[i].forceActive = forceActive;
        }
    }
}
