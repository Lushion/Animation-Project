using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    /* GAMEOBJECTS FOR IMAGE RENDERING */ 

    private Canvas canvas;
    public Image image;
    public string fileName;

    public bool showCharacter = false;
    public bool showContour = true;
    public bool showMesh = true;

    /* JOINT AND LIMB NAMES */

    public static readonly string[] jointNames = new string[]
    {
        "0. Nose",
        "1. Neck",
        "2. Left Shoulder",
        "3. Left Elbow",
        "4. Left Wrist",
        "5. Right Shoulder",
        "6. Right Elbow",
        "7. Right Wrist",
        "8. Center Hip",
        "9. Left Hip",
        "10. Left Knee",
        "11. Left Ankle",
        "12. Right Hip",
        "13. Right Knee",
        "14. Right Ankle",
        "15. Left Eye",
        "16. Right Eye",
        "17. Left Ear",
        "18. Right Ear",
        "19. Right Big Toe",
        "20. Right Small Toe",
        "21. Right Heel",
        "22. Left Big Toe",
        "23. Left Small Toe",
        "24. Left Heel",
        "25. Half Spine"
    };

    public static readonly string[] limbNames = new string[]
    {
        "Head",
        "Spine",
        "Left Upper Arm",
        "Right Upper Arm",
        "Left Forearm",
        "Right Forearm",
        "Hips and Upper Legs",
        "Left Lower Leg",
        "Right Lower Leg",
    };

    // Directed Graph
    // Numbers represent the index of a joint in jointNames
    public static readonly Dictionary<int, int[]> jointHierarchy = new Dictionary<int, int[]>
        {
            { 0, new int[]{ 15, 16 } },
            { 8, new int[]{ 25, 9, 12 } },
            { 25, new int[]{ 1 } },
            { 1, new int[]{ 0, 2, 5 } },
            { 2, new int[]{ 3 } },
            { 3, new int[]{ 4 } },
            { 5, new int[]{ 6 } },
            { 6, new int[]{ 7 } },
            { 9, new int[]{ 10 } },
            { 10, new int[]{ 11 } },
            { 11, new int[]{ 22, 23, 24 } },
            { 12, new int[]{ 13 } },
            { 13, new int[]{ 14 } },
            { 14, new int[]{ 19, 20, 21 } },
            { 15, new int[]{ 17 } },
            { 16, new int[]{ 18 } },
        };

    // Relationship between limbs and joints
    // Numbers represent the index of a joint in jointNames
    public static readonly Dictionary<string, int[]> limbJointsDependencies = new Dictionary<string, int[]>()
    {
        { "Head", new int[]{ 1, 0 } },
        { "Spine", new int[]{ 8, 1, 2, 5 } },
        { "Left Upper Arm", new int[]{ 2, 3 } },
        { "Right Upper Arm", new int[]{ 5, 6 } },
        { "Left Forearm", new int[]{ 3, 4 } },
        { "Right Forearm", new int[]{ 6, 7 } },
        { "Hips and Upper Legs", new int[]{ 8, 9, 10, 12, 13 } },
        //{ "Left Upper Leg", new int[]{ 9, 10 } },
        { "Left Lower Leg", new int[]{ 10, 11 } },
        //{ "Right Upper Leg", new int[]{ 12, 13 } },
        { "Right Lower Leg", new int[]{ 13, 14 } },
    };

    private List<Character> characters = new List<Character>();
    public Character character;


    /*  ----  */
    /*  MAIN  */
    /*  ----  */

    void Start()
    {
        /*  CANVAS  */

        // Creates a canvas to host the image
        GameObject canvasGO = new GameObject();
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.name = "Canvas";
        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.AddComponent<CanvasScaler>();



        //Vector2 position = new Vector2(canvas.pixelRect.width / 2, canvas.pixelRect.height / 2);
        Vector2 position = new Vector2(0, 0);

        // Shows the original image
        GameObject imageGO = new GameObject();
        image = imageGO.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Images/" + fileName);
        image.rectTransform.sizeDelta = new Vector2(image.sprite.rect.width, image.sprite.rect.height);
        imageGO.transform.SetParent(canvas.transform);
        imageGO.transform.localScale = new Vector3(0.1f, 0.1f);
        imageGO.transform.localPosition = position;
        imageGO.name = "Sprite";

        Camera.main.orthographicSize = image.sprite.rect.height / 20;

        // Creates a JSON reader
        JsonReader jr = gameObject.AddComponent<JsonReader>();
        jr.fileName = fileName;
        List<Vector2> jointCoordinates = ArrangeJointCoordinates(jr.ReadJointPositions());
        List<List<Vector3>> contourList = ArrangeContourCoordinates(jr.ReadLimbContourPoints());

        // Creates a character
        GameObject characterGO = new GameObject();
        character = characterGO.AddComponent<Character>();

        character.transform.SetParent(canvas.transform);
        character.transform.localPosition = position;
        character.name = "Character";

        character.SetJoints(jointCoordinates);
        character.SetLimbs(contourList);
        character.SetRig();

        CharacterAnimation animation = gameObject.AddComponent<CharacterAnimation>();
        animation.Setup(character, canvas);

    }

    public void Update()
    {
        ShowGraphics();
        //character.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }


    /*  METHODS  */

    public void ShowGraphics()
    {
        image.enabled = showCharacter;
        foreach (Limb limb in character.limbs) { 
            limb.mr.enabled = showMesh;
            limb.contour.mr.enabled = showContour;
            limb.contour.thickness = image.sprite.rect.height / 1000;
        }
    }


    public List<Vector2> ArrangeJointCoordinates(List<float> jointCoordList)
    {
        List<Vector2> coordUnity = new List<Vector2>();

        // Set the coordinates of the joints
        for (int index = 0; index < jointCoordList.Count / 3; index++)
        {
            Vector2 jointCoordOP = new Vector2(jointCoordList[3 * index], jointCoordList[3 * index + 1]);
            Vector2 jointCoordUnity = ConvertCoordinates(jointCoordOP);
            coordUnity.Add(jointCoordUnity/10);
        }

        // Adds a joint for the Spine
        // "1" refers to the neck; "8" refers to the hips (see jointNames)
        Vector2 HalfSpineCoord = new Vector2((coordUnity[1].x + coordUnity[8].x) / 2, (coordUnity[1].y + coordUnity[8].y) / 2);
        coordUnity.Add(HalfSpineCoord);

        return coordUnity;
    }


    public List<List<Vector3>> ArrangeContourCoordinates(List<List<Vector2>> contourList)
    {
        List<List<Vector3>> contourListUnity = new List<List<Vector3>>();

        // Set the coordinates of the contour points
        foreach (List<Vector2> contour in contourList)
        {
            List<Vector3> coordUnity = new List<Vector3>();

            foreach (Vector3 point in contour)
            {
                coordUnity.Add(new Vector3(ConvertCoordinates(point).x/10, ConvertCoordinates(point).y/10, 0) );
            }

            contourListUnity.Add(coordUnity);
        }

        return contourListUnity;
    }


    public Vector2 ConvertCoordinates(Vector2 ptOpenPose)
    {
        // Openpose coordinates: (0, 0) at top left
        // Unity coordinates: (0, 0) at the center of the image from bottom to top
        // The vertical axis needs to be reversed and the image needs to be centered 
        // The canvas dimensions are offsets
        float x = ptOpenPose.x - (image.sprite.rect.width / 2);
        float y = -ptOpenPose.y + (image.sprite.rect.height / 2);

        Vector2 ptUnity = new Vector2(x, y);

        return ptUnity;
    }
}
