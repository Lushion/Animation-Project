using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimation : MonoBehaviour
{
    public Canvas canvas;
    public Character character;
    public List<CharacterJoint> Joints { get { return character.joints; } }
         
    public List<CharacterPose> keyPoses = new List<CharacterPose>();
    public List<int> frameNumber = new List<int>();
    public float timing = 2;
    public int framePerSecond = 24;
    public int selectedKeyPose;
    
    public Button poseRegistrationButton = null;
    public Button characterPoseSelectionButton = null;
    public Button deletionButton = null;
    public Button modificationButton = null;
    public Button animationButton = null;
    public Text text = null;
    public InputField iF = null;

    public void Setup(Character character, Canvas canvas)
    {
        this.character = character;
        this.canvas = canvas;
        //RegisterPose();

        GameObject UI = new GameObject { name = "UI Elements" };
        UI.transform.SetParent(canvas.transform);

        GameObject regButtonGO = DefaultControls.CreateButton(new DefaultControls.Resources());
        regButtonGO.transform.SetParent(UI.transform);
        poseRegistrationButton = regButtonGO.GetComponent<Button>();
        poseRegistrationButton.GetComponentInChildren<Text>().text = "Register Pose";
        poseRegistrationButton.onClick.AddListener(() => { RegisterPose(); });

        GameObject charaButtonGO = DefaultControls.CreateButton(new DefaultControls.Resources());
        charaButtonGO.transform.SetParent(UI.transform);
        characterPoseSelectionButton = charaButtonGO.GetComponent<Button>();
        characterPoseSelectionButton.onClick.AddListener(() => { SetCharacterToPose(); });

        GameObject delButtonGO = DefaultControls.CreateButton(new DefaultControls.Resources());
        delButtonGO.transform.SetParent(UI.transform);
        deletionButton = delButtonGO.GetComponent<Button>();
        deletionButton.onClick.AddListener(() => { DeletePose(); });


        GameObject modifButtonGO = DefaultControls.CreateButton(new DefaultControls.Resources());
        modifButtonGO.transform.SetParent(UI.transform);
        modificationButton = modifButtonGO.GetComponent<Button>();
        modificationButton.onClick.AddListener(() => { ModifyPose(); });


        GameObject animButtonGO = DefaultControls.CreateButton(new DefaultControls.Resources());
        animButtonGO.transform.SetParent(UI.transform);
        animationButton = animButtonGO.GetComponent<Button>();
        animationButton.GetComponentInChildren<Text>().text = "Animate";
        animationButton.onClick.AddListener(() => { RoutineWrap(); });

        GameObject textGO = DefaultControls.CreateText(new DefaultControls.Resources());
        textGO.transform.SetParent(UI.transform);
        text = textGO.GetComponent<Text>();

        GameObject ifGO = DefaultControls.CreateInputField(new DefaultControls.Resources());
        ifGO.transform.SetParent(UI.transform);
        iF = ifGO.GetComponent<InputField>();
        iF.transform.position = new Vector3(canvas.pixelRect.width / 2, canvas.pixelRect.height + iF.gameObject.GetComponent<RectTransform>().rect.height / 2, 0);
        iF.contentType = InputField.ContentType.IntegerNumber;
        iF.text = "0";
    }


    public void Update()
    {
        // Set positions
        RectTransform regRect = poseRegistrationButton.GetComponent<RectTransform>();
        poseRegistrationButton.transform.position = new Vector3(regRect.rect.width / 2, canvas.pixelRect.height - regRect.rect.height / 2, 0);

        RectTransform charaRect = characterPoseSelectionButton.GetComponent<RectTransform>();
        characterPoseSelectionButton.transform.position = new Vector3(charaRect.rect.width / 2, canvas.pixelRect.height - charaRect.rect.height * 3/ 2, 0);
        characterPoseSelectionButton.GetComponentInChildren<Text>().text = "Set To Selected Pose " + selectedKeyPose.ToString();

        RectTransform delRect = deletionButton.GetComponent<RectTransform>();
        deletionButton.transform.position = new Vector3(delRect.rect.width / 2, canvas.pixelRect.height - delRect.rect.height * 5 / 2, 0);
        deletionButton.GetComponentInChildren<Text>().text = "Remove Pose " + selectedKeyPose.ToString();

        RectTransform modRect = modificationButton.GetComponent<RectTransform>();
        modificationButton.transform.position = new Vector3(modRect.rect.width / 2, canvas.pixelRect.height - modRect.rect.height * 7 / 2, 0);
        modificationButton.GetComponentInChildren<Text>().text = "Modify Pose " + selectedKeyPose.ToString();

        RectTransform animRect = animationButton.GetComponent<RectTransform>();
        animationButton.transform.position = new Vector3(canvas.pixelRect.width - animRect.rect.width / 2, canvas.pixelRect.height - animRect.rect.height / 2, 0);

        text.transform.position = new Vector3(canvas.pixelRect.width / 2, canvas.pixelRect.height - text.rectTransform.rect.height * 3 / 2, 0);
        iF.transform.position = new Vector3(canvas.pixelRect.width / 2, canvas.pixelRect.height - iF.gameObject.GetComponent<RectTransform>().rect.height / 2, 0);


        // REmove unnecessary buttons
        characterPoseSelectionButton.image.enabled = (selectedKeyPose < keyPoses.Count);
        deletionButton.image.enabled = (selectedKeyPose < keyPoses.Count);
        modificationButton.image.enabled = (selectedKeyPose < keyPoses.Count);

        characterPoseSelectionButton.transform.GetComponentInChildren<Text>().enabled = (selectedKeyPose < keyPoses.Count);
        deletionButton.transform.GetComponentInChildren<Text>().enabled = (selectedKeyPose < keyPoses.Count);
        modificationButton.transform.GetComponentInChildren<Text>().enabled = (selectedKeyPose < keyPoses.Count);

        try
        {
            selectedKeyPose = int.Parse(iF.textComponent.text);

        }
        catch (FormatException)
        {
            print("Input could not be parsed");
        }

        if (selectedKeyPose < keyPoses.Count)
        {
            text.text = "Selected Key Pose: " + selectedKeyPose;
        }
        else
        {
            text.text = "Key Pose " + selectedKeyPose + " doesn't exist";
        }
    }


    public void RegisterPose() 
    {
        CharacterPose characterPose = new CharacterPose();



        foreach (Limb limb in character.limbs)
        {
            foreach (Spring spring in limb.contour.springs)
            {
                spring.handleA.transform.localPosition = spring.start.position;
                spring.handleB.transform.localPosition = spring.mobile.position;

                characterPose.handles.Add(spring, new List<Vector3>() { spring.handleA.transform.localPosition, spring.handleB.transform.localPosition } );
            }
        }

        for (int i = 0; i < Joints.Count; i++)
        {
            characterPose.pose.Add(Joints[i].transform.localPosition);
        }

        print("Registered Joint Positions");
        keyPoses.Add(characterPose);
    }

    public void SetCharacterToPose()
    {
        if (keyPoses[selectedKeyPose] != null)
        {

            foreach (Limb limb in character.limbs)
            {
                foreach (Spring spring in limb.contour.springs)
                {
                    spring.handleA.transform.localPosition = keyPoses[selectedKeyPose].handles[spring][0];
                    spring.handleB.transform.localPosition = keyPoses[selectedKeyPose].handles[spring][1];
                }
            }
            for (int i = 0; i < Joints.Count; i++)
            {
                Joints[i].point.position = keyPoses[selectedKeyPose].pose[i];
            }


            print("Reverted to Pose " + selectedKeyPose.ToString());

        }

        else
        {
            print("Pose not registered");
        }
    }

    public void DeletePose()
    {
        if (keyPoses[selectedKeyPose] != null)
        {
            keyPoses.Remove(keyPoses[selectedKeyPose]);
            
            print("Removed Pose " + selectedKeyPose.ToString());

        }

        else
        {
            print("Pose not registered");
        }
    }

    public void ModifyPose()
    {
        if (keyPoses[selectedKeyPose] != null)
        {

            foreach (Limb limb in character.limbs)
            {
                foreach (Spring spring in limb.contour.springs)
                {
                    spring.handleA.transform.position = spring.start.position;
                    spring.handleB.transform.position = spring.mobile.position;

                    keyPoses[selectedKeyPose].handles[spring][0] = spring.handleA.transform.localPosition;
                    keyPoses[selectedKeyPose].handles[spring][1] = spring.handleB.transform.localPosition;
                }
            }

            for (int i = 0; i < Joints.Count; i++)
            {
                keyPoses[selectedKeyPose].pose[i] = Joints[i].transform.localPosition;
            }


            print("Changed Pose " + selectedKeyPose.ToString());

        }

        else
        {
            print("Pose not registered");
        }
    }

    public IEnumerator Translate()
    {
        for (int keyPose = 0; keyPose < keyPoses.Count - 1; keyPose++)
        {
            float t = 0;
            for (float frame = 0; frame < framePerSecond; frame++)
            {
                float s = 0;
                for(int i = 0; i < framePerSecond; i++)
                {
                    s += (float)i / (float)framePerSecond * (1 - (float)i / (float)framePerSecond);
                }

                while (t < 1)
                {
                    float lastT = t;
                    t += frame / (float)framePerSecond * (1 - frame / (float)framePerSecond) / s;


                    Vector3 lastPosition = character.transform.position + Vector3.Lerp(keyPoses[keyPose].pose[8], keyPoses[keyPose + 1].pose[8], Mathf.Max(lastT, 0));
                    Joints[8].transform.localPosition = Vector2.Lerp(keyPoses[keyPose].pose[8], keyPoses[keyPose + 1].pose[8], t);


                    Vector3 normal = Joints[8].transform.position - lastPosition;
                    Vector3 velocity = normal / framePerSecond;

                    Vector3 tilt = Vector3.up;
                    if (!float.IsNaN(velocity.x)) { tilt = velocity + Mathf.Abs(1 / (t - lastT)) * Vector3.up; }

                    Joints[8].transform.rotation = Quaternion.LookRotation(Vector3.forward, tilt);


                    yield return new WaitForSeconds(1 / framePerSecond);
                }
            }

            yield return new WaitForSeconds(1);
            Joints[8].transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            yield return new WaitForSeconds(1);
            StopCoroutine(Translate());
        }
    }

    public IEnumerator IK()
    {
        for (int keyPose = 0; keyPose < keyPoses.Count - 1; keyPose++)
        {
            float t = 0;
            for (float frame = 0; frame < framePerSecond; frame++)
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
                    foreach (IK ik in character.iks)
                    {
                        ik.SetupPositions(keyPoses[keyPose].pose[ik.id - 1], keyPoses[keyPose].pose[ik.id], keyPoses[keyPose + 1].pose[ik.id - 1], keyPoses[keyPose + 1].pose[ik.id]);

                        if (ik.root != null && ik.inbound != null && ik.endEff != null)
                        {
                            ik.RotationAnalytics(t);
                        }
                    }
                    yield return new WaitForSeconds(timing / framePerSecond);
                }


            }

            StopCoroutine(Translate());
        }
    }

    public void RoutineWrap()
    {
        //StartCoroutine(Translate());
        StartCoroutine(IK());
    }



    public class CharacterPose
    {
        public List<Vector3> pose = new List<Vector3>(); // position of all the joints
        public Dictionary<Spring, List<Vector3>> handles = new Dictionary<Spring, List<Vector3>>(); // handles of the springs
    }

}
