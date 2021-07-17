using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MeshCreator
{
    /* FIELDS */

    // Lists
    public Character character;
    public LimbContour contour;
    public List<CharacterJoint> joints;
    public IK ik;

    public Vector3 Direction { get { return joints[1].gameObject.transform.position - joints[0].gameObject.transform.position; } }


    /* METHODS */

    public virtual void Update()
    {
        CreateMesh();
    }

    public void Set(Character character, string name, int[] jointIndexes, List<Vector3> contourPoints)
    {
        this.character = character;
        this.name = name;

        joints = new List<CharacterJoint>();
        foreach(int index in jointIndexes)
        {
            joints.Add(character.FindJoint(index));
        }
        
        
        // Get every X points (X = step)
        int step = 30;
        for (int i = 0; i < (contourPoints.Count / step); i++)
        {
            meshPoints.Add(contourPoints[step * i]);
        }

        // Creates a Limb Game Object in the hierarchy as a child of Limbs
        gameObject.transform.SetParent(character.meshGO.transform);

        // Show contour
        GameObject contourGO = new GameObject { name = name };
        contour = contourGO.AddComponent<LimbContour>();
        contour.Setup(this);

        // Renders the mesh
        if (mr == null)
        {
            mr = gameObject.AddComponent<MeshRenderer>();
            material = Resources.Load<Material>("Skin");
            mr.materials[0] = material;
            mr.enabled = false;
        }
    }



    // Updates the Vertices, Normals, UVs and Tirangles lists at every change of index
    public override void UpdateLists()
    {
        for (int index = 0; index < ((N / step) / 2); index++)
        {
            // Indexes of the 4 points taken for each quad
            int a = step * (index + 1);
            int b = step * (index);
            int c = ((N / step) - index) * step - 1;
            int d = ((N / step) - index - 1) * step - 1;

            CreateQuadMesh(meshPoints[a], meshPoints[b], meshPoints[c], meshPoints[d], index);
        }
    }
}
