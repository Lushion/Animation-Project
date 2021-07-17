using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterMesh : MonoBehaviour
{
    public List<Vector3> contourPoints;

    public List<Vector3> vertices;
    public List<Edge> edges;
    public List<Vector3> triangles;
    public float minimumAngle = 20;

    public LimbContour contour;
    public List<Vector3> ContourPoint { get { return contour.meshPoints; } }
    public List<CharacterJoint> Joints { get { return contour.Joints; } }

}
