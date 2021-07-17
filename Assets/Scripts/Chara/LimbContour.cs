using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimbContour : ContourStretchSquash
{
    /*  FIELDS  */

    //// Contour parameters
    //float length;

    // Limb
    public Limb limb;
    public List<CharacterJoint> Joints { get { return limb.joints; } }
    public List<Vector3> MeshPoints { get { return limb.meshPoints; } }

    /// <summary>
    /// Removing the Contour Stretsh Squash testing Start funciton
    /// </summary>
    public override void Start()
    {
        
    }

    /// <summary>
    /// Same as Contour Stretch Squash Update function, but also updating limb's contour and mesh
    /// </summary>
    public override void Update()
    {
        for (int i = 0; i < SplinePoints.Count; i++)
        {
            foreach (Spring spring in springs)
            {
                spring.forceActive = forceActive;
            }
            
            if (Joints.Count == 2 || Joints.Count == 4)
            {
                SplinePoints[i].position = sts1[i].C.position;
                MeshPoints[i] = sts1[i].C.position;
            }


            if (Joints.Count == 3 || Joints.Count == 5)
            {
                SplinePoints[i].position = sts1[i].C.position;
                MeshPoints[i] = sts1[i].C.position;
            }
        }

        if (SplinePoints.Count > 0)
        {
            DrawSpline();
        }
    }

    public void Setup(Limb limb)
    {
        // Create a Spring 
        this.limb = limb;
        transform.SetParent(limb.character.contoursGO.transform);
        points = limb.meshPoints;

        SetSprings();

        foreach (Spring spring in springs)
        {
            
            spring.handleA.sr.enabled = false;
            spring.handleA.isTied = true;
            spring.handleA.isFixed = false;
            
        }

        // Renders the mesh
        thickness = 2f;
        mr = gameObject.AddComponent<MeshRenderer>();
        material = Resources.Load<Material>("Line");
        mr.material = material;
    }


    public void SetSprings()
    {
        // Normal limbs or spine
        if (Joints.Count == 2 || Joints.Count == 4)
        {
            spring = gameObject.AddComponent<Spring>();
            spring.Setup(Joints[0].point, Joints[1].point);
            springs.Add(spring);

            foreach (Vector3 point in points)
            {
                Vertex vertex = new Vertex(point);
                SplinePoints.Add(vertex);

                StretchSquashTriangle st = gameObject.AddComponent<StretchSquashTriangle>();
                st.Setup(Joints[0].point, Joints[1].point, new Vertex(point));
                sts1.Add(st);
            }

            // Spine
            if (Joints.Count == 4)
            {
                StretchSquashTriangle phantomST1 = gameObject.AddComponent<StretchSquashTriangle>();
                phantomST1.Setup(Joints[0].point, Joints[1].point, Joints[2].point);
                sts1.Add(phantomST1);


                StretchSquashTriangle phantomST2 = gameObject.AddComponent<StretchSquashTriangle>();
                phantomST2.Setup(Joints[0].point, Joints[1].point, Joints[3].point);
                sts1.Add(phantomST2);
            }

        }

        // Spine and half-spine
        else if (Joints.Count == 3)
        {
            Spring spring1 = gameObject.AddComponent<Spring>();
            spring1.Setup(Joints[0].point, Joints[1].point);
            springs.Add(spring1);
            spring.handleA.sr.enabled = false;
            spring.handleA.isTied = false;
            spring.handleA.isFixed = false;

            Spring spring2 = gameObject.AddComponent<Spring>();
            spring2.Setup(Joints[1].point, Joints[2].point);
            springs.Add(spring2);
            spring.handleA.sr.enabled = false;
            spring.handleA.isTied = false;
            spring.handleA.isFixed = false;


            foreach (Vector3 point in points)
            {
                Vertex vertex = new Vertex(point);
                SplinePoints.Add(vertex);


                if (Vector3.Distance(point, Joints[0].point.position) < Vector3.Distance(point, Joints[2].point.position))
                {
                    StretchSquashTriangle st = gameObject.AddComponent<StretchSquashTriangle>();
                    st.Setup(Joints[0].point, Joints[1].point, new Vertex(point));
                    sts1.Add(st);
                }

                else
                {
                    StretchSquashTriangle st = gameObject.AddComponent<StretchSquashTriangle>();
                    st.Setup(Joints[1].point, Joints[2].point, new Vertex(point));
                    sts1.Add(st);
                }
            }
        }

        // Hips
        else if (Joints.Count == 5)
        {
            Spring spring1 = gameObject.AddComponent<Spring>();
            spring1.Setup(Joints[1].point, Joints[2].point);
            springs.Add(spring1);


            Spring spring2 = gameObject.AddComponent<Spring>();
            spring2.Setup(Joints[3].point, Joints[4].point);
            springs.Add(spring2);

            foreach (Vector3 point in points)
            {
                Vertex vertex = new Vertex(point);
                SplinePoints.Add(vertex);

                StretchSquashTriangle st = gameObject.AddComponent<StretchSquashTriangle>();
                if (Vector3.Distance(point, Joints[1].point.position) < Vector3.Distance(point, Joints[3].point.position))
                {
                    st.Setup(Joints[1].point, Joints[2].point, new Vertex(point));
                }

                else
                {
                    st.Setup(Joints[3].point, Joints[4].point, new Vertex(point));
                }

                sts1.Add(st);
            }


            Spring phantomSpring1 = gameObject.AddComponent<Spring>();
            phantomSpring1.Setup(Joints[0].point, Joints[1].point);
            phantomSpring1.handleA.sr.enabled = false;
            phantomSpring1.handleA.isTied = true;
            phantomSpring1.handleA.isFixed = false;
            phantomSpring1.handleB.sr.enabled = false;
            phantomSpring1.handleB.isTied = true;
            phantomSpring1.handleB.isFixed = false;
            phantomSpring1.maxStretch = 0;


            Spring phantomSpring2 = gameObject.AddComponent<Spring>();
            phantomSpring2.Setup(Joints[0].point, Joints[3].point);

            phantomSpring2.handleA.sr.enabled = false;
            phantomSpring2.handleA.isTied = true;
            phantomSpring2.handleA.isFixed = false;
            phantomSpring2.handleB.sr.enabled = false;
            phantomSpring2.handleB.isTied = true;
            phantomSpring2.handleB.isFixed = false;
            phantomSpring2.maxStretch = 0;
        }

    }
}
