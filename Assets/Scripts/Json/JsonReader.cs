using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JsonReader : MonoBehaviour
{
    /*  FILENAMES  */

    // Open Pose Keypoints
    string jointsFileName = "Assets/Resources/Jsons/DSC00944_keypoints.json";

    // SCHP's Mask's contours
    string contoursFileName = "Assets/Resources/Jsons/contours.json";


    /*  CHARACTER RIG */

    public List<float> ReadJointPositions()
    {
        string json = File.ReadAllText(jointsFileName);
        JsonContent jsonContent = JsonUtility.FromJson<JsonContent>(json);

        List<float> jointPositions = new List<float>();
        foreach (Person person in jsonContent.people)
        {
            jointPositions = person.pose_keypoints_2d;
        }

        return jointPositions;
    }


    /*  LIMBS & CONTOURS  */

    public List<List<Vector2>> ReadLimbContourPoints()
    {
        string LimbContour = File.ReadAllText(contoursFileName);
        ContourJsonContent LimbContourJsonContent = JsonUtility.FromJson<ContourJsonContent>(LimbContour);
        List<List<Vector2>> contourList = new List<List<Vector2>>();

        // Reading the JSON output
        foreach (ContourJson limbContourJson in LimbContourJsonContent.contours)
        {
            List<Vector2> contourPointsUnity = new List<Vector2>();

            foreach (Vector2 contourPointOpenPose in limbContourJson.contourPoints)
            {
                contourPointsUnity.Add(contourPointOpenPose);
            }

            contourList.Add(contourPointsUnity);
        }

        return contourList;
    }



    /*  TEMPORARY CLASSES  */

    [Serializable]
    class JsonContent
    {
        public float version;
        public Person[] people;
    }


    [Serializable]
    class Person
    {
        // float[] List<float>
        public List<float> person_id;
        public List<float> pose_keypoints_2d;
        public List<float> face_keypoints_2d;
        public List<float> hand_left_keypoints_2d;
        public List<float> hand_right_keypoints_2d;
        public List<float> pose_keypoints_3d;
        public List<float> face_keypoints_3d;
        public List<float> hand_left_keypoints_3d;
        public List<float> hand_right_keypoints_3d;
    }

    [Serializable]
    class ContourJsonContent
    {
        public ContourJson[] contours;
    }

    [Serializable]
    class ContourJson
    {
        // float[] List<float>
        public List<Vector2> contourPoints;
        public int contourLength;
    }

}
