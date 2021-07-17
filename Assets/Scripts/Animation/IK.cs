using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IK : MonoBehaviour
{
    public int id;
    public Transform root;
    public Transform inbound;    
    public Transform endEff;    // End Effector

    public Character character;
    public Spring RI;
    public Spring IE;

    // Local Positions
    public Vector3 initInbJointPos;
    public Vector3 initEndEffPos;
    public Vector3 finInbJointPos;
    public Vector3 finEndEffPos;

    // Global Positions

    public bool ellipse = true;
    public bool linear = false;

    public bool test = false;
    public float t = 0;
    public float frame = 0;
    public float framePerSecond = 12;

    public void Start()
    {
        if (test)
        {
            StartCoroutine(IKtest());

        }

    }

    public void Update()
    {
        if (test)
        {
            Debug.DrawLine(root.position, inbound.position, Color.black);
            Debug.DrawLine(inbound.position, endEff.position, Color.black);
            Debug.DrawLine(root.position, endEff.position, Color.blue);
            Debug.DrawLine((root.position + endEff.position) / 2, inbound.position, Color.blue);

        }
    }

    public IEnumerator IKtest()
    {
        while (test)
        {

            float s = 0;
            for (int i = 0; i < framePerSecond; i++)
            {
                s += (float)i / (float)framePerSecond * (1 - (float)i / (float)framePerSecond);
            }

            while (t < 1)
            {
                t += frame / (float)framePerSecond * (1 - frame / (float)framePerSecond) / s;
                frame++;
                // Lines, for testing
                if (root != null && inbound != null && endEff != null)
                {
                    RotationAnalytics(t);
                }
                yield return new WaitForSeconds(1/framePerSecond);
            }

            t = 0; frame = 0; 

        }
    }
    public void SetupTransforms(int endEffIndex, Character character)
    {
        // Sets the transforms of the game objects
        id = endEffIndex;

        foreach (Limb limb in character.limbs)
        {
            if (limb.joints.Contains(character.FindJoint(endEffIndex)) && limb.joints.Contains(character.FindJoint(endEffIndex - 1)))
            {
                foreach (Spring spring in limb.contour.springs)
                {
                    if (spring.mobile == character.FindJoint(endEffIndex).point)
                    {
                        IE = spring;
                    }
                }
            }
            else if (limb.joints.Contains(character.FindJoint(endEffIndex-2)) && limb.joints.Contains(character.FindJoint(endEffIndex - 1)))
            {
                
                foreach (Spring spring in limb.contour.springs)
                {
                    if (spring.mobile == character.FindJoint(endEffIndex-1).point)
                    {
                        RI = spring;
                    }
                }
            }
        }

        endEff = IE.handleB.transform;
        inbound = RI.handleB.transform;
        root = RI.handleA.transform;

        if (id == 4)
        {
            print(id);
            print(endEff.position);
            print(inbound.position);
            print(root.position);
        }
    }

    public void SetupPositions(Vector3 initInbound, Vector3 initEndEff, Vector3 targetInbound, Vector3 targetEndEffector)
    {

        // Sets the initial and final positions
        initEndEffPos = initEndEff;
        initInbJointPos = initInbound;
        finEndEffPos = targetEndEffector;
        finInbJointPos = targetInbound;
    }


    public void RotationAnalytics(float t)
    {
        // Find the target
        Vector3 currentInboundTarget = TweenInterpolationEllipseInb(t);
        if (linear) { currentInboundTarget = TweenLinearInterpolationInb(t); }
        Vector3 currentEndTarget = TweenInterpolationEllipse(t);
        if (linear) { currentEndTarget = TweenLinearInterpolation(t); }


        float a = (currentInboundTarget - root.position).magnitude;    // Length Root - Mid
        float b = (currentEndTarget - currentInboundTarget).magnitude;     // Length Mid - Tip
        float c = Vector3.Distance(root.position, currentEndTarget); // Length Root - Target

        // Sets useful angles
        //float angleRoot = CosineRule(b, a, c);
        //float angleBasis = Angle.GetXYBasisRotationAngle(currentTarget - root.position);
        //float angleMid = CosineRule(c, a, b);

        //// Rotations using quaternions
        //root.rotation = Quaternion.Euler(x: 0, y: 0, z: angleRoot + angleBasis);
        //inbound.localPosition = new Vector3(a, 0, 0);

        //inbound.localRotation = Quaternion.Euler(x: 0, y: 0, z: 180 + angleMid);
        //endEff.localPosition = new Vector3(b, 0, 0);

        //// Rotations using local euler (don't work yet) 
        //root.localEulerAngles = new Vector3(root.localEulerAngles.x, root.localEulerAngles.y, angleRoot + angleBasis);
        //inbound.localEulerAngles = new Vector3(inbound.localEulerAngles.x, inbound.localEulerAngles.y, 180 + angleMid);

        //inbound.localRotation = Quaternion.Euler(x: 0, y: 0, z: 180 + angleMid);
        //endEff.localPosition = new Vector3(b, 0, 0);

        // Rotation using projections
        float angleRoot = CosineRule(b, a, c);
        if (Angle.CCW(new Vertex(root.position), new Vertex(currentInboundTarget), new Vertex(currentEndTarget)))
        {
            angleRoot = -CosineRule(b, a, c);
        }
        float angleBasis = Angle.GetXYBasisRotationAngle(currentEndTarget - root.position);
        inbound.position = new Vector3(a * Mathf.Cos((angleBasis + angleRoot) * Mathf.Deg2Rad), a * Mathf.Sin((angleBasis + angleRoot) * Mathf.Deg2Rad), 0) + root.position;


        float angleMid = CosineRule(c, a, b);
        float angleBasis2 = Angle.GetXYBasisRotationAngle(currentEndTarget - inbound.position);
        endEff.position = currentEndTarget;
        //endEff.position = new Vector3(c * Mathf.Cos((180 + angleMid + angleBasis + angleRoot) * Mathf.Deg2Rad), c * Mathf.Sin((180 + angleMid + angleBasis + angleRoot) * Mathf.Deg2Rad), 0) + inbound.position;
    }


    /* Triangle Calculation */

    public float CosineRule(float b, float a, float c)
    {
        float cosB = ((a * a) + (c * c) - (b * b)) / (2 * a * c);


        if (!float.IsNaN(Mathf.Acos(cosB)))
        {
            return Mathf.Round(Mathf.Acos(cosB) * Mathf.Rad2Deg * 100) * 0.01f;
        }
        else { return 1; }
    }


    /* INTERPOLATION */

    Vector3 TweenLinearInterpolation(float t)
    {
        return Vector3.Lerp(initEndEffPos - initInbJointPos, finEndEffPos - finInbJointPos, t);
    }
    Vector3 TweenLinearInterpolationInb(float t)
    {
        return Vector3.Lerp(initInbJointPos - root.position, finInbJointPos - root.position, t);
    }
    Vector3 TweenInterpolationEllipse(float t)
    {
        if (initEndEffPos != finEndEffPos)
        {
            Vector3 a = 2 * initEndEffPos - 2 * finEndEffPos + Mathf.Sign((finEndEffPos - initEndEffPos).y) * Vector3.up + Mathf.Sign((finEndEffPos - initEndEffPos).x) * Vector3.right;
            Vector3 b = -3 * initEndEffPos + 3 * finEndEffPos - 2 * Mathf.Sign((finEndEffPos - initEndEffPos).y) * Vector3.up - Mathf.Sign((finEndEffPos - initEndEffPos).x) * Vector3.right;
            Vector3 c = Mathf.Sign((finEndEffPos - initEndEffPos).y) * Vector3.up;
            Vector3 d = initEndEffPos;

            return a * t * t * t + b * t * t + c * t + d;

        }

        else { return initEndEffPos; }
    }

    Vector3 TweenInterpolationEllipseInb(float t)
    {
        if (initInbJointPos != finInbJointPos)
        {
            Vector3 a = 2 * initInbJointPos - 2 * finInbJointPos + Mathf.Sign((finInbJointPos - initInbJointPos).y) * Vector3.up + Mathf.Sign((finInbJointPos - initInbJointPos).x) * Vector3.right;
            Vector3 b = -3 * initInbJointPos + 3 * finInbJointPos - 2 * Mathf.Sign((finInbJointPos - initInbJointPos).y) * Vector3.up - Mathf.Sign((finInbJointPos - initInbJointPos).x) * Vector3.right;
            Vector3 c = Mathf.Sign((finInbJointPos - initInbJointPos).y) * Vector3.up;
            Vector3 d = initInbJointPos;

            return a * t * t * t + b * t * t + c * t + d;

        }

        else { return initInbJointPos; }
    }
}
