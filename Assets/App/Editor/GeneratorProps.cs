using UnityEditor;
using UnityEngine;

public enum Type
{
    None = 0,
    Bridge  = 1,
    Hammer = 2,
}

public class GeneratorProps : EditorWindow
{
    [SerializeField] Type type = Type.None;
    [SerializeField] private GameObject objectToPlace = null;
    [SerializeField] int number = 5;
    [SerializeField] float spacing = 1;

    private Vector2 scrollPosition = Vector2.zero;
    private GUIStyle labelStyle;
    private GUIStyle labelStyleSelected;
    private GUIStyle buttonGenerateBridgeStyle;

    [MenuItem("Tools/Generator Props")]
    public static void ShowWindow()
    {
        // Create the UI
        GeneratorProps window = GetWindow<GeneratorProps>("Generator Props");
        window.minSize = new Vector2(400, 400);
        window.maxSize = new Vector2(400, 400);

        window.position = new Rect(100, 100, 400, 400);
    }

    private void SetupStyles()
    {
        labelStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(10, 10, 10, 5),
        };

        buttonGenerateBridgeStyle = new GUIStyle(GUI.skin.button)
        {
            margin = new RectOffset(30, 30, 10, 5),
            padding = new RectOffset(10, 10, 15, 15),
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            fontSize = 20
        };
    }

    private void OnGUI()
    {
        SetupStyles();

        GUILayout.Label("Generator Props", labelStyle);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Type");
        type = (Type)EditorGUILayout.EnumPopup("Type", type);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (type == Type.Bridge)
        {
            GUIBridge();
        }
        else if (type == Type.Hammer)
        {
            GUIHammer();
        }

        EditorGUILayout.EndScrollView();

        EditorGUI.BeginDisabledGroup(type == Type.None || objectToPlace == null || number < 1 || spacing < 0);
        if (GUILayout.Button("GENERATE", buttonGenerateBridgeStyle))
        {
            Generate();
        }
        EditorGUI.EndDisabledGroup();
    }

    private void GUIBridge()
    {
        if (objectToPlace == null)
        {
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("GameObject to Place is Empty!", MessageType.Error);
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Bridge Object");
        objectToPlace = (GameObject)EditorGUILayout.ObjectField("GameObject to Place", objectToPlace, typeof(GameObject), false);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Bridge Number");
        number = Mathf.Clamp(EditorGUILayout.IntField("Number", number), 1, 100);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Bridge Space");
        spacing = Mathf.Clamp(EditorGUILayout.FloatField("Spacing", spacing), 0, 100);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
    }

    private void GUIHammer()
    {

    }

    private void Generate()
    {
        if (type == Type.Bridge)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            Camera sceneCamera = sceneView.camera;
            Vector3 spawnPosition = sceneCamera.transform.position + sceneCamera.transform.forward * 5f;

            // Create Parent
            GameObject bridgeParent = new GameObject("Bridge");
            Undo.RegisterCreatedObjectUndo(bridgeParent, "Create Bridge");
            bridgeParent.transform.position = spawnPosition;
            bridgeParent.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

            // Create Children
            for (int i = 0; i < number; i++)
            {
                Vector3 position = bridgeParent.transform.position - new Vector3(i * spacing, 0, 0);

                GameObject newSegment = Instantiate(objectToPlace, position, Quaternion.identity);
                newSegment.transform.SetParent(bridgeParent.transform);
                newSegment.transform.Rotate(0, 0, 90);
            }
        }
        else if (type == Type.Hammer)
        {

        }
    }
}