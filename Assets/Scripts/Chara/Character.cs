using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    /*  Game Objects  */
    public GameObject meshGO;
    public GameObject contoursGO;
    public GameObject rigGO;

    /*  Dictionaries */

    public List<CharacterJoint> joints = new List<CharacterJoint>();
    public List<Limb> limbs = new List<Limb>();
    public List<IK> iks = new List<IK>();

    public static string[] JointNames { get { return CharacterCreator.jointNames; } }
    public static string[] LimbNames { get { return CharacterCreator.limbNames; } }
    public static Dictionary<int, int[]> JointHierarchy { get { return CharacterCreator.jointHierarchy; } }
    public static Dictionary<string, int[]> LimbJointsDependencies { get { return CharacterCreator.limbJointsDependencies; } }


    /*  METHODS  */

    public void SetLimbs(List<List<Vector3>> contourList)
    {
        meshGO = new GameObject();
        meshGO.transform.SetParent(transform);
        meshGO.name = "Mesh";
        meshGO.transform.localPosition = new Vector3(0, 0, 0);

        contoursGO = new GameObject();
        contoursGO.transform.SetParent(transform);
        contoursGO.name = "Contour";
        contoursGO.transform.localPosition = new Vector3(0, 0, 0);

        int counter = 0;
        foreach (KeyValuePair<string, int[]> kvp in LimbJointsDependencies)
        {
            GameObject limbGO = new GameObject();
            Limb limb = limbGO.AddComponent<Limb>();

            limb.Set(this, kvp.Key, kvp.Value, contourList[counter]);
            limbs.Add(limb);
            counter++;
        }
    }


    public void SetJoints(List<Vector2> jointCoordList)
    {
        // Creates the joints
        for (int index = 0; index < jointCoordList.Count; index++)
        {
            GameObject jointGO = new GameObject { name = JointNames[index] };
            CharacterJoint joint = jointGO.AddComponent<CharacterJoint>();
            joint.SetJoint(this, index, jointCoordList[index]);
            joints.Add(joint);
        }

        // Structures the joints
        SetBoneStructure();
        //ShowBoneStructure();
    }

    public void SetBoneStructure()
    {
        // Creates a Game Object Root that stores the Armature
        GameObject rootGO = new GameObject { name = "Armature Root" };
        rootGO.transform.SetParent(transform);

        // Assigns the central hips to the root object
        FindJoint(8).gameObject.transform.SetParent(rootGO.transform);


        // Sets the armature from Graph
        foreach (int parentIndex in JointHierarchy.Keys)
        {
            CharacterJoint jointParent = FindJoint(parentIndex);

            if (JointHierarchy[parentIndex].Length > 1) // If there is more than 1 child object
            {
                foreach (int childIndex in JointHierarchy[parentIndex])
                {
                    // Creates an object that serves as a link between parent and one of the children
                    GameObject tempChildGO = new GameObject();
                    tempChildGO.transform.SetParent(jointParent.gameObject.transform);
                    tempChildGO.transform.localPosition = new Vector2(0, 0);

                    CharacterJoint jointChild = FindJoint(childIndex);
                    jointChild.gameObject.transform.SetParent(tempChildGO.transform);
                    tempChildGO.name = "Basis " + jointParent.id + jointChild.id;
                }
            }

            else
            {
                CharacterJoint jointChild = FindJoint(JointHierarchy[parentIndex][0]);
                jointChild.gameObject.transform.SetParent(jointParent.gameObject.transform);
            }
        }

    }

    public void SetRig()
    {
        rigGO = new GameObject { name = "Rig" };
        Rig rig = rigGO.AddComponent<Rig>();
        rig.transform.SetParent(transform);

        //////IK spineIK = CreateNewIkRig("Spine", 0);
        CreateNewIkRig("Head", 0, 1, 8);
        CreateNewIkRig("Left Arm", 4, 3, 2);
        CreateNewIkRig("Right Arm", 7, 6, 5);

        CreateNewIkRig("Left Leg", 11, 10, 9);
        CreateNewIkRig("Right Leg", 14, 13, 12);
    }

    public IK CreateNewIkRig(string name, int endEffectorIndex, int inbIndex, int rootIndex)
    {
        GameObject IKGO = new GameObject();
        IKGO.transform.SetParent(rigGO.transform);
        IKGO.name = name;

        CharacterJoint joint = FindJoint(endEffectorIndex);
        IK ik = IKGO.AddComponent<IK>();
        ik.SetupTransforms(endEffectorIndex, inbIndex, rootIndex, this);
        iks.Add(ik);
        
        return ik;
    }

    public void ShowBoneStructure()
    {
        // Adds a Components for Bone Rendering
        RigBuilder rb = gameObject.AddComponent<RigBuilder>();
        BoneRenderer boneRenderer = gameObject.AddComponent<BoneRenderer>();

        // Creates a list of transforms for the bone renderer
        boneRenderer.transforms = new Transform[joints.Count];

        for (int i = 0; i < joints.Count; i++)
        {
            boneRenderer.transforms[i] = FindJoint(i).gameObject.transform;
        }
    }

    public CharacterJoint FindJoint(int index) { return joints[index]; }

}

