using UnityEngine;

public class Spring : MonoBehaviour
{
    // Heads of the spring
    public Vertex start; // fixed head of the spring
    public Vertex mobile;   // mobile head of the spring
    public Vertex equilibirum;  // initial position of the mobile head of the spring
    //public Vector3 A;
    //public Vector3 B;

    // Force applied on the spring
    public GameObject handleAGO;
    public GameObject handleBGO;
    public Handle handleA;
    public Handle handleB;

    public bool forceActive;
    Vector3 amplitude;  // amplitude of the movement equation

    public Vector3 Force { get { return handleB.transform.position - equilibirum.position; } }

    // Direction of the spring
    Vector3 Direction { get { return (handleB.transform.position - handleA.transform.position).normalized; } }

    // Physical parameters
    float timer = 0;
    float k { get { return 1 / equilibirumLength; } }
    float mu { get { return 1 / (4 * equilibirumLength); } }
    readonly float m = 0.001f;

    float omega0 { get { return Mathf.Sqrt(k / m); } }
    float delta { get { return mu / (2 * m); } }

    public float equilibirumLength;
    public float maxStretch;

    //// For Testing Purposes
    //public void Start()
    //{
    //    start = new Vertex(A);
    //    mobile = new Vertex(B);
    //    equilibirum = new Vertex(B);
    //    equilibirumLength = Vector3.Distance(equilibirum.position, start.position);
    //    forceActive = true;

    //    if (handleAGO == null)
    //    {
    //        handleAGO = new GameObject();
    //        handleA = handleAGO.AddComponent<Handle>();
    //        handleA.Setup(start);
    //        handleA.isFixed = true;
    //    }

    //    if (handleBGO == null)
    //    {
    //        handleBGO = new GameObject();
    //        handleB = handleBGO.AddComponent<Handle>();
    //        handleB.Setup(mobile);
    //    }
    //}

    public void Setup(Vertex fixedPoint, Vertex mobilePoint)
    {
        start = fixedPoint;
        mobile = mobilePoint;
        equilibirum = mobilePoint;
        equilibirumLength = Vector3.Distance(equilibirum.position, start.position);
        maxStretch = equilibirumLength/5;
        forceActive = true;

        if (handleAGO == null) 
        { 
            handleAGO = new GameObject { name = name + " A" };
            handleA = handleAGO.AddComponent<Handle>();
            handleA.transform.SetParent(transform);
            handleA.isFixed = true;
        }

        if (handleBGO == null) 
        { 
            handleBGO = new GameObject { name = name + " B" };
            handleB = handleBGO.AddComponent<Handle>();
            handleB.transform.SetParent(transform);
        }

        handleA.Setup(start);
        handleB.Setup(mobile);
    }

    void Update()
    {
        if (handleB != null && forceActive)
        {
            // Make sure that the resting position is always at resting legth from the fixedPoint
            equilibirum.position = handleA.transform.position + equilibirumLength * Direction;

            float selectedForceMagnitude = (handleB.transform.position - handleA.transform.position).magnitude;

            if (selectedForceMagnitude - equilibirumLength >= maxStretch)
            {
                mobile.position = (equilibirumLength + maxStretch) * Direction.normalized + handleA.transform.position;
                amplitude = maxStretch * Direction.normalized;
            }

            else if (selectedForceMagnitude - equilibirumLength <= -maxStretch)
            {
                mobile.position = (equilibirumLength - maxStretch) * Direction.normalized + handleA.transform.position;
                amplitude = -maxStretch * Direction.normalized;
            }

            else
            {
                mobile.position = selectedForceMagnitude * Direction.normalized + handleA.transform.position;
                amplitude = (selectedForceMagnitude - equilibirumLength) * Direction.normalized;
            }

            timer = 0;
        }

        else if (Mathf.Abs(Vector3.Distance(handleA.transform.position, mobile.position)) != equilibirumLength)
        {
            timer += Time.deltaTime;
            mobile.position = calcDampedHarmonic(timer);
        }

        // Debug.DrawLine(handleA.transform.position, mobile.position);
        
    }


    /// <Summary>
    /// Finds the position of the modile head of the spring if the object is oscillating. 
    /// Damped Harmonic Equation.
    /// </Summary>
    /// <param name="force"> A handle that mimics a force applied on the spring </param>
    /// <param name="t"> Time </param>
    /// <returns> </returns>
    Vector3 calcDampedHarmonic(float t)
    {
        Vector3 mobileExt = handleA.transform.position + equilibirumLength * Direction + amplitude * Mathf.Exp(-delta * t) * Mathf.Cos(omega0 * t);

        // Stability reached
        if (timer > 5)
        {
            mobileExt = handleA.transform.position + equilibirumLength * Direction;
            timer = 0;
        }

        return mobileExt;
    }
}
